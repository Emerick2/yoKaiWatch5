#!/usr/bin/env python3
"""Export the extracted COLLADA map tiles as one clean Wavefront OBJ scene.

The game DAE files contain a few malformed triangle declarations which make the
current Blender COLLADA extension abort.  This exporter reads their geometry
directly, bakes the COLLADA node transforms, and skips only degenerate faces.
It deliberately leaves the original files untouched.

Example:
  python3 Tools/export_collada_map.py \
    --input-dir 'Assets/2 - map 3D/GEN2/t101' --output-dir Exports/t101
"""
from __future__ import annotations

import argparse
import math
import os
from pathlib import Path
import re
import xml.etree.ElementTree as ET


NS = {"c": "http://www.collada.org/2005/11/COLLADASchema"}
EPSILON = 1e-10


def tag(element: ET.Element, name: str) -> str:
    return f"{{{NS['c']}}}{name}"


def floats(text: str | None) -> list[float]:
    return [float(v) for v in (text or "").split()]


def integers(text: str | None) -> list[int]:
    return [int(v) for v in (text or "").split()]


def identity() -> list[list[float]]:
    return [[1.0 if row == col else 0.0 for col in range(4)] for row in range(4)]


def multiply(left: list[list[float]], right: list[list[float]]) -> list[list[float]]:
    return [[sum(left[row][k] * right[k][col] for k in range(4)) for col in range(4)] for row in range(4)]


def collada_matrix(values: list[float]) -> list[list[float]]:
    # COLLADA serializes its matrices column-major.
    return [[values[col * 4 + row] for col in range(4)] for row in range(4)]


def transform_point(matrix: list[list[float]], point: tuple[float, float, float]) -> tuple[float, float, float]:
    x, y, z = point
    return (
        matrix[0][0] * x + matrix[0][1] * y + matrix[0][2] * z + matrix[0][3],
        matrix[1][0] * x + matrix[1][1] * y + matrix[1][2] * z + matrix[1][3],
        matrix[2][0] * x + matrix[2][1] * y + matrix[2][2] * z + matrix[2][3],
    )


def node_matrix(node: ET.Element) -> list[list[float]]:
    result = identity()
    for child in node:
        if child.tag == tag(child, "matrix"):
            values = floats(child.text)
            if len(values) == 16:
                result = multiply(result, collada_matrix(values))
        elif child.tag == tag(child, "translate"):
            x, y, z = floats(child.text)[:3]
            transform = identity()
            transform[0][3], transform[1][3], transform[2][3] = x, y, z
            result = multiply(result, transform)
        elif child.tag == tag(child, "scale"):
            x, y, z = floats(child.text)[:3]
            transform = identity()
            transform[0][0], transform[1][1], transform[2][2] = x, y, z
            result = multiply(result, transform)
    return result


def safe_name(value: str) -> str:
    return re.sub(r"[^A-Za-z0-9_.-]+", "_", value).strip("_") or "material"


def parse_materials(root: ET.Element, dae: Path, output: Path) -> dict[str, str]:
    images = {image.get("id"): (image.findtext("c:init_from", namespaces=NS) or "")
              for image in root.findall(".//c:library_images/c:image", NS)}
    effects: dict[str, str] = {}
    for effect in root.findall(".//c:library_effects/c:effect", NS):
        params = {}
        for newparam in effect.findall(".//c:newparam", NS):
            sid = newparam.get("sid")
            surface = newparam.find("c:surface/c:init_from", NS)
            sampler = newparam.find("c:sampler2D/c:source", NS)
            if sid and surface is not None:
                params[sid] = (surface.text or "").strip()
            if sid and sampler is not None:
                params[sid] = (sampler.text or "").strip()
        diffuse = effect.find(".//c:profile_COMMON//c:diffuse/c:texture", NS)
        texture = ""
        if diffuse is not None:
            texture = params.get(diffuse.get("texture", ""), "")
            texture = params.get(texture, texture)
            texture = images.get(texture, texture)
        effects[effect.get("id", "")] = texture

    materials = {}
    for material in root.findall(".//c:library_materials/c:material", NS):
        effect = material.find("c:instance_effect", NS)
        texture = effects.get((effect.get("url", "") if effect is not None else "").lstrip("#"), "")
        material_name = safe_name(material.get("id", "material"))
        if texture:
            texture_path = (dae.parent / texture).resolve()
            try:
                # map_Kd paths are evaluated relative to the MTL file itself.
                texture = os.path.relpath(texture_path, output)
            except ValueError:
                texture = str(texture_path)
        materials[material.get("id", "")] = (material_name, texture)
    return materials


def read_geometry(root: ET.Element) -> dict[str, list[tuple[str, list[tuple[tuple[float, float, float], tuple[float, float]]]]]]:
    geometries = {}
    for geometry in root.findall(".//c:library_geometries/c:geometry", NS):
        mesh = geometry.find("c:mesh", NS)
        if mesh is None:
            continue
        sources = {}
        for source in mesh.findall("c:source", NS):
            array = source.find("c:float_array", NS)
            accessor = source.find(".//c:accessor", NS)
            if array is None:
                continue
            stride = int(accessor.get("stride", "1")) if accessor is not None else 1
            sources[f"#{source.get('id')}"] = (floats(array.text), stride)
        vertices = {}
        for vertex in mesh.findall("c:vertices", NS):
            position = vertex.find("c:input[@semantic='POSITION']", NS)
            if position is not None:
                vertices[f"#{vertex.get('id')}"] = position.get("source")
        primitives = []
        for triangles in mesh.findall("c:triangles", NS):
            inputs = [(entry.get("semantic"), entry.get("source"), int(entry.get("offset", "0")))
                      for entry in triangles.findall("c:input", NS)]
            width = max((entry[2] for entry in inputs), default=-1) + 1
            data = integers(triangles.findtext("c:p", default="", namespaces=NS))
            faces = []
            for start in range(0, len(data) - width * 3 + 1, width * 3):
                corners = []
                for corner in range(3):
                    indices = data[start + corner * width:start + (corner + 1) * width]
                    position = uv = None
                    for semantic, source, offset in inputs:
                        if offset >= len(indices):
                            continue
                        if semantic == "VERTEX":
                            source = vertices.get(source, source)
                            values, stride = sources.get(source, ([], 0))
                            index = indices[offset] * stride
                            if index + 2 < len(values):
                                position = (values[index], values[index + 1], values[index + 2])
                        elif semantic == "TEXCOORD" and uv is None:
                            values, stride = sources.get(source, ([], 0))
                            index = indices[offset] * stride
                            if index + 1 < len(values):
                                uv = (values[index], values[index + 1])
                    if position is not None:
                        corners.append((position, uv or (0.0, 0.0)))
                if len(corners) == 3:
                    faces.append(tuple(corners))
            primitives.append((triangles.get("material", "default"), faces))
        geometries[f"#{geometry.get('id')}"] = primitives
    return geometries


def read_controllers(root: ET.Element) -> dict[str, str]:
    """Map skin controllers to their source geometry.

    These map exports wrap every static object in a skin controller, often
    with zero vertex weights.  For a static export the controller's source
    mesh is the visual data; retaining the armature would only add clutter.
    """
    result = {}
    for controller in root.findall(".//c:library_controllers/c:controller", NS):
        skin = controller.find("c:skin", NS)
        if skin is not None and skin.get("source"):
            result[f"#{controller.get('id')}"] = skin.get("source")
    return result


def instance_materials(instance: ET.Element) -> dict[str, str]:
    return {entry.get("symbol", "default"): entry.get("target", "").lstrip("#")
            for entry in instance.findall(".//c:instance_material", NS)}


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("--input-dir", required=True, type=Path)
    parser.add_argument("--output-dir", required=True, type=Path)
    args = parser.parse_args()
    input_dir, output_dir = args.input_dir.resolve(), args.output_dir.resolve()
    output_dir.mkdir(parents=True, exist_ok=True)
    obj_path, mtl_path = output_dir / "map_clean.obj", output_dir / "map_clean.mtl"
    dae_files = sorted(input_dir.rglob("*.dae"))
    if not dae_files:
        raise SystemExit(f"No DAE found under {input_dir}")

    material_defs: dict[str, tuple[str, str]] = {}
    faces_by_material: dict[str, list[tuple[tuple[float, float, float], tuple[float, float]]]] = {}
    report = []
    for dae in dae_files:
        root = ET.parse(dae).getroot()
        material_defs.update(parse_materials(root, dae, output_dir))
        geometries = read_geometry(root)
        controllers = read_controllers(root)
        before = sum(len(faces) // 3 for faces in faces_by_material.values())

        def walk(node: ET.Element, parent: list[list[float]]) -> None:
            world = multiply(parent, node_matrix(node))
            for instance in node.findall("c:instance_geometry", NS):
                bindings = instance_materials(instance)
                for symbol, faces in geometries.get(instance.get("url", ""), []):
                    material = bindings.get(symbol, symbol)
                    key = material if material in material_defs else "default"
                    target = faces_by_material.setdefault(key, [])
                    for face in faces:
                        points = [transform_point(world, corner[0]) for corner in face]
                        a = tuple(points[1][i] - points[0][i] for i in range(3))
                        b = tuple(points[2][i] - points[0][i] for i in range(3))
                        area_squared = sum(value * value for value in (a[1] * b[2] - a[2] * b[1], a[2] * b[0] - a[0] * b[2], a[0] * b[1] - a[1] * b[0]))
                        if len(set(points)) == 3 and area_squared > EPSILON:
                            target.extend((point, uv) for point, (_, uv) in zip(points, face))
            for instance in node.findall("c:instance_controller", NS):
                bindings = instance_materials(instance)
                geometry_id = controllers.get(instance.get("url", ""), "")
                for symbol, faces in geometries.get(geometry_id, []):
                    material = bindings.get(symbol, symbol)
                    key = material if material in material_defs else "default"
                    target = faces_by_material.setdefault(key, [])
                    for face in faces:
                        points = [transform_point(world, corner[0]) for corner in face]
                        a = tuple(points[1][i] - points[0][i] for i in range(3))
                        b = tuple(points[2][i] - points[0][i] for i in range(3))
                        area_squared = sum(value * value for value in (a[1] * b[2] - a[2] * b[1], a[2] * b[0] - a[0] * b[2], a[0] * b[1] - a[1] * b[0]))
                        if len(set(points)) == 3 and area_squared > EPSILON:
                            target.extend((point, uv) for point, (_, uv) in zip(points, face))
            for child in node.findall("c:node", NS):
                walk(child, world)

        scene = root.find(".//c:library_visual_scenes/c:visual_scene", NS)
        if scene is not None:
            for node in scene.findall("c:node", NS):
                walk(node, identity())
        after = sum(len(faces) // 3 for faces in faces_by_material.values())
        report.append(f"{dae.relative_to(input_dir)}: {after - before} triangles")

    with mtl_path.open("w", encoding="utf-8") as mtl:
        for material in sorted(faces_by_material):
            name, texture = material_defs.get(material, (safe_name(material), ""))
            mtl.write(f"newmtl {name}\nKd 1.000 1.000 1.000\n")
            if texture and Path(output_dir / texture).exists():
                mtl.write(f"map_Kd {texture.replace(os.sep, '/')}\n")
            mtl.write("\n")
    vertex_index = uv_index = 1
    with obj_path.open("w", encoding="utf-8") as obj:
        obj.write(f"mtllib {mtl_path.name}\no map_clean\n")
        for material, corners in sorted(faces_by_material.items()):
            obj.write(f"usemtl {material_defs.get(material, (safe_name(material), ''))[0]}\n")
            for point, uv in corners:
                obj.write(f"v {point[0]:.7g} {point[1]:.7g} {point[2]:.7g}\n")
                obj.write(f"vt {uv[0]:.7g} {uv[1]:.7g}\n")
            for index in range(0, len(corners), 3):
                obj.write(f"f {vertex_index}/{uv_index} {vertex_index + 1}/{uv_index + 1} {vertex_index + 2}/{uv_index + 2}\n")
                vertex_index += 3
                uv_index += 3
    (output_dir / "export_report.txt").write_text("\n".join(report) + "\n", encoding="utf-8")
    print(f"Exported {sum(len(corners) // 3 for corners in faces_by_material.values())} triangles to {obj_path}")


if __name__ == "__main__":
    main()

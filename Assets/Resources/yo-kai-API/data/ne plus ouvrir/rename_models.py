import json
import os

listeModele = ["","y001000","y001800_p00","y101000","y102000","y102010","y103000","y104000","y104010","y105000","y105010","y106000","y107000","y107010","y108000","y109000","y177000","y109010","y110000","y111000","y112000","y112010","y113000","y113010","y113020","y114000","y115000","y115010","y116000","y116010","y117000","y117010","y118000","y118010","y119000","y119010","y120000","y120010","y121010","y122000","y122010","y123000","y124000","y124010","y125000","y125010","y126000","y127000","y127010","y128000","y129000","y129010","y130000","y130010","y131000","y131010","y131020","y132000","y133000","y133010","y133800","y134000","y134010","y134020","y135000","y136000","y136010","y137000","y137010","y138000","y139000","y139010","y140000","y141000","y141010","y142000","y142010","y143000","y144000","y144010","y145000","y146000","y146010","y147000","y147010","y147020","y147030","y148000","y148010","y149000","y150000","y150010","y151000","y151010","y151020","y152000","y152900","y153000","y154000","y155000","y155010","y156000","y156010","y156020","y156030","y156040","y157000","y158000","y158010","y159000","y159010","y159400","y159900","y159910","y160000","y160020","y161000","y161010","y162000","y163000","y163010","y164000","y164010","y165000","y165010","y166000","y166010","y167000","y167010","y168000","y169000","y170000","y170010","","y171000","y171010","y172000","y172010","y173000","y173010","y174000","y174010","y174020","y175000","y176000","y176010","y178000","y178010","y179000","y179010","y180000","y180010","y181000","y181010","y182000","y183000","y183010","y184000","y185000","y185010","y186000","y186010","y187000","y187010","y188000","y188010","y189000","y190000","y190020","y191000","y191010","y192000","y193000","y193010","y194000","y194010","y195000","y195010","y196000","y196010","y197000","y198000","y198010","y199000","y199010","y199020","y200000","y200010","y201000","y202000","y202010","y203000","y204000","y204010","y205000","y205010","","y206000","y206010","y207000","y207020","y208000","y209000","y209010","y210000","y210010","y211000","y211010","y212000","y213000","y214000","y214010","y215000","y216000","y216010","y217000","y217010","y218000","y218010","y219000","y219010","y220000","y220010","y220020","y221000","y221010","y221020","y222000","y223000","y224000","y225000","y226000","y231000","y232000","y233000","y234000","y235000","","y236000","y236900","y237000","","y238000","y239000","y240000","y241000","y242000","y242010","y243000","y244000","y244000","y245000","y246000","y247000","y248000","y249000","","y250000","y251000","y252000","y253000","y253010","y254000","y255000","y256000","y257000","y258000","y259000","y260000","y261000","y262000","y262010","y262010","y263000","y264000","y265000","y266000","y266010","y267000","y267000","y268000","y268000","y269000","y270000","y271000","y272000","y273000","y274000","y274010","","y275000","y276000","y277000","","y278000","","y280000","","y281000","","y282000","y283000","y284000","y285000","y286000","y287000","y288000","y289000","y290000","y290010","y291000","","y292000","y293000","y294000","y295000","y296000","y297000","","y299000","y300000","y301000","","y303000","y303500","y304000","","y305000","y306000","y307000","y308000","y309000","y310000","y311000","","y312000","y313000","y313010","y314000","y315000","y316000","y317000","y318000","","y320000","y321000","","y322000","","y323000","y324000","y325000","y326000","y327000","y327010","y329000","","y331000","","","","","","","","","y340000","y341000","y342000","y343000","y344000","y345000","y346000","y347000","y348000","y349000","","","","y353000","y354000","y355000","y401000","","y402000","y403000","y404000","y405000","y406000","y407000","y408000","y409000","y410000","y411000","y412000","y413000","y414000","y415000","y416000","y417000","y418000","y371000","y372000","y373000","y374000","y375000","y376000","y377000","y377010","y377020","y377030","y377040","y377050","y451000","y452000","y453000","y454000","y455000","y456000","y457000","y458000","y459000","y472000","y471000","y473000","y474000","y475000","y476000","y477000","y478000","","","y481000","","y483000","y484000","","y486000","","","","y501000","y502000","y503000","y504000","y505000","y506000","y507000","y508000","y509000","y510000","y511000","y512000","y512010","y513000","y514000","y515000","y516000","y517000","y518000","y519000","y520000","y521000","y522000","y523000","y524000","y525000","y526000","y527000","y528000","y529000","y530000","y531000","y532000","y533000","y534000","y535000","y536000","y536010","y537000","y538000","y539000","y540000","y541000","y542000","y543000","y544000","y545000","y546000","y547000","y548000","y548100","y548110","y548120","y549000","y550000","y551000","y552000","y552600","y553000","y554000","y554010","y555000","y556000","y557000","y558000","y559000","y560000","y561000","y562000","y563000","y564000","y565000","y566000","y567000","y568000","y569000","y570000","y570010","y571000","y571010","y572000","y573000","y574000","y575000","y576000","y577000","y578000","y579000","y580000","y581000","y582000","y583000","y584000","y585000","y586000","y587000","y588000","y589000","y589010","y590000","y591000","y592000","y593000","y594000","y595000","y596000","y597000","y598000","y599000","y600000","y601000","y602000","y603000","y604000","y605000","y606000","y607000","y608000","y609000","y609010","y610000","y611000","y612000","y613000","y614000","y615000","y616000","y617000","y618000","y619000","y620000","y621000","y622000","y623000","y624000","y625000","y626000","y627000","y628000","y628010","y629000","y630000","y631000","y632000","y633000","y634000","y635000","y636000","y637000","y638000","y639000","y640000","y641000","y642000","y643000","y644000","y645000","y646000","y646010","y647000","y648000","y649000","y650000","y661000","","","","","","","","y667000","y668000","y669000","y671000","y672000","y673000","y674000","y675000","y676000","y677000","y678000","y679000","y680000","","","y681000","y682000","y683000","y684000","y685000","y686000","y687000","y688000","y689000","y690000","y691000","y692000","","","y695000","y696000","","y698000","y699000","y701000","y702000","y703000","y704000","y705000","y706000","y707000","y708000","y711000","y712000","y713000","y714000","y715000","y716000","y717000","y720000","y721000","y722000","y723000","y724000","y725000","y726000","","y728000","y729000","y730000","y731000","y732000","y733000","y734000","y735000","y736000","","y738000","y739000","y740000","y741000","y742000","y743000","y744000","y745000","y746000","y747000","y748000","","","","y752000","y753000","y754000","y755000","y756000","y761000","y762000","y763000","y764000","y765000","y766000","y767000","y771000","y772000","y773000","y774000","y775000","y776000","y777000","y778000","y779000","y780000","y781000","y782000","y783000","y801000","y802000","y803000","y804000","y805000","y806000","y807000","y808000","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","","",""]
automatique = True
i = 540

def rename_model():
    """
    Renomme un modèle 3D, ses textures et met à jour le fichier de mapping JSON.
    """
    global i
    print(i)
    script_dir = os.path.dirname(os.path.abspath(__file__))
    json_path = os.path.join(script_dir, '_3D', 'yokai_assets_mapping.json')
    models_dir = os.path.join(script_dir, '_3D', 'models')
    textures_dir = os.path.join(script_dir, '_3D', 'textures')

    while True:
        i+=1
        try:
            with open(json_path, 'r', encoding='utf-8') as f:
                mapping_data = json.load(f)
        except (FileNotFoundError, json.JSONDecodeError) as e:
            print(f"Erreur: Impossible de lire le fichier {json_path}. ({e})")
            return

        old_id = ""
        new_id = ""
        if automatique == False :
            old_id = input("Entrez l'ID actuel du modèle (ex: 101000) ou 'q' pour quitter: ")
            if old_id.lower() == 'q':
                break
            new_id = input(f"Entrez le nouvel ID pour '{old_id}': ")
        else :
            if len(listeModele) > i :
                if listeModele[i] == "" :
                    continue
                else :
                    old_id = listeModele[i]
                    old_id = old_id.replace("y","")
                    new_id = i
            else :
                return

        if old_id not in mapping_data:
                print(f"Erreur: L'ID '{old_id}' n'a pas été trouvé dans {json_path}.")
                continue
        if not new_id:
            print("Le nouvel ID ne peut pas être vide.")
            continue

        if new_id in mapping_data:
            confirm = input(f"Attention: L'ID '{new_id}' existe déjà. Voulez-vous l'écraser ? (o/n): ").lower()
            if confirm != 'o':
                print("Opération annulée.")
                continue
        
        # Préparation des nouveaux noms de fichiers
        model_info = mapping_data[old_id]
        old_model_file = model_info.get("modelFile")
        new_model_file = f"model_y{new_id}.fbx" if old_model_file else None

        new_texture_files = []
        if "textureFiles" in model_info and model_info["textureFiles"]:
            for i, old_texture_file in enumerate(model_info["textureFiles"]):
                # Garde le suffixe original s'il y en a un (ex: _face, _eye)
                parts = os.path.splitext(old_texture_file)
                base_name = parts[0]
                extension = parts[1]
                
                suffix_parts = base_name.split(f"y{old_id}")
                suffix = suffix_parts[1] if len(suffix_parts) > 1 else f"_{i}"

                new_texture_files.append(f"texture_y{new_id}{suffix}{extension}")
        
        # Renommage des fichiers
        try:
            # Renommer le modèle
            if old_model_file:
                old_model_path = os.path.join(models_dir, old_model_file)
                new_model_path = os.path.join(models_dir, new_model_file)
                if os.path.exists(old_model_path):
                    os.rename(old_model_path, new_model_path)
                    print(f"Modèle renommé : {old_model_path} -> {new_model_path}")
                else:
                    print(f"Avertissement: Fichier modèle {old_model_path} non trouvé.")

            # Renommer les textures
            if "textureFiles" in model_info:
                for i, old_texture_file in enumerate(model_info["textureFiles"]):
                    old_texture_path = os.path.join(textures_dir, old_texture_file)
                    new_texture_path = os.path.join(textures_dir, new_texture_files[i])
                    if os.path.exists(old_texture_path):
                        os.rename(old_texture_path, new_texture_path)
                        print(f"Texture renommée : {old_texture_path} -> {new_texture_path}")
                    else:
                        print(f"Avertissement: Fichier texture {old_texture_path} non trouvé.")

        except OSError as e:
            print(f"Erreur lors du renommage des fichiers : {e}")
            continue

        # Mise à jour du JSON
        new_entry = {
            "modelFile": new_model_file,
            "textureFiles": new_texture_files
        }
        
        # Créer une nouvelle copie pour éviter les problèmes de modification pendant l'itération
        updated_mapping_data = {k: v for k, v in mapping_data.items() if k != old_id}
        updated_mapping_data[new_id] = new_entry

        try:
            with open(json_path, 'w', encoding='utf-8') as f:
                json.dump(updated_mapping_data, f, indent=2)
            print(f"Le fichier {json_path} a été mis à jour avec succès.")
        except IOError as e:
            print(f"Erreur lors de l'écriture dans {json_path}: {e}")

        print("-" * 30)

if __name__ == "__main__":
    rename_model()

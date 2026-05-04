import os
import tkinter as tk
from tkinter import filedialog, messagebox, ttk
from PIL import Image, ImageDraw, ImageChops

class AutoCircleApp:
    def __init__(self, root):
        self.root = root
        self.root.title("Découpage Yo-kai (Transparence Totale)")
        self.root.geometry("500x320")
        self.root.resizable(False, False)

        self.dossier_source = tk.StringVar()
        self.dossier_destination = tk.StringVar()

        self.creer_interface()

    def creer_interface(self):
        frame = ttk.Frame(self.root, padding=20)
        frame.pack(fill=tk.BOTH, expand=True)

        ttk.Label(frame, text="Traitement par lots : Cercle + Alpha", font=("Arial", 12, "bold")).pack(pady=(0, 20))

        # Dossier Source
        ttk.Label(frame, text="Dossier des images (carrées avec fond transparent) :").pack(anchor=tk.W)
        f_src = ttk.Frame(frame)
        f_src.pack(fill=tk.X, pady=(0, 10))
        ttk.Entry(f_src, textvariable=self.dossier_source, state='readonly').pack(side=tk.LEFT, fill=tk.X, expand=True, padx=(0, 5))
        ttk.Button(f_src, text="Ouvrir", command=self.choisir_source).pack(side=tk.RIGHT)

        # Dossier Destination
        ttk.Label(frame, text="Dossier de sauvegarde :").pack(anchor=tk.W)
        f_dst = ttk.Frame(frame)
        f_dst.pack(fill=tk.X, pady=(0, 20))
        ttk.Entry(f_dst, textvariable=self.dossier_destination, state='readonly').pack(side=tk.LEFT, fill=tk.X, expand=True, padx=(0, 5))
        ttk.Button(f_dst, text="Ouvrir", command=self.choisir_destination).pack(side=tk.RIGHT)

        self.progbar = ttk.Progressbar(frame, orient=tk.HORIZONTAL, mode='determinate')
        self.progbar.pack(fill=tk.X, pady=(0, 10))

        self.btn_demarrer = ttk.Button(frame, text="🚀 Lancer l'automatisation", command=self.traiter_images)
        self.btn_demarrer.pack(fill=tk.X)

    def choisir_source(self):
        d = filedialog.askdirectory()
        if d: self.dossier_source.set(d)

    def choisir_destination(self):
        d = filedialog.askdirectory()
        if d: self.dossier_destination.set(d)

    def traiter_images(self):
        src, dst = self.dossier_source.get(), self.dossier_destination.get()
        if not src or not dst:
            messagebox.showwarning("Erreur", "Veuillez choisir les deux dossiers.")
            return

        fichiers = [f for f in os.listdir(src) if f.lower().endswith(('.png', '.webp'))]
        if not fichiers:
            messagebox.showinfo("Info", "Aucun fichier PNG/WebP trouvé.")
            return

        self.btn_demarrer.config(state=tk.DISABLED)
        self.progbar['maximum'] = len(fichiers)

        for i, fichier in enumerate(fichiers):
            try:
                # 1. Charger l'image en RGBA
                img = Image.open(os.path.join(src, fichier)).convert("RGBA")
                largeur, hauteur = img.size
                
                # 2. Extraire la transparence originale (le masque alpha actuel)
                alpha_original = img.split()[3]

                # 3. Créer le masque du cercle (en haute définition pour lisser les bords)
                facteur = 4
                masque_cercle_hd = Image.new('L', (largeur * facteur, hauteur * facteur), 0)
                draw = ImageDraw.Draw(masque_cercle_hd)
                draw.ellipse((0, 0, largeur * facteur, hauteur * facteur), fill=255)
                masque_cercle = masque_cercle_hd.resize((largeur, hauteur), Image.LANCZOS)

                # 4. FUSION : On multiplie le masque du cercle par la transparence d'origine
                # Seuls les pixels qui sont à la fois DANS le cercle ET OPAQUES à la base resteront visibles.
                nouvel_alpha = ImageChops.multiply(alpha_original, masque_cercle)

                # 5. Appliquer le résultat final
                img.putalpha(nouvel_alpha)
                
                # Sauvegarde au même nom en PNG
                img.save(os.path.join(dst, os.path.splitext(fichier)[0] + ".png"), "PNG")

            except Exception as e:
                print(f"Erreur sur {fichier}: {e}")

            self.progbar['value'] = i + 1
            self.root.update_idletasks()

        self.btn_demarrer.config(state=tk.NORMAL)
        messagebox.showinfo("Succès", f"{len(fichiers)} médailles créées avec fond transparent préservé !")

if __name__ == "__main__":
    root = tk.Tk()
    app = AutoCircleApp(root)
    root.mainloop()
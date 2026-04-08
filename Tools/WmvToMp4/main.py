import os
import ffmpeg

FOLDER_Z_FILMAMI = r'D:\multimedia_do_pytan _expo'

def convert_and_cleanup():
    if not os.path.exists(FOLDER_Z_FILMAMI):
        print(f"Błąd: Folder {FOLDER_Z_FILMAMI} nie istnieje.")
        return

    files = [f for f in os.listdir(FOLDER_Z_FILMAMI) if f.lower().endswith('.wmv')]
    print(f"Znaleziono {len(files)} plików do konwersji z kompresją.")

    for filename in files:
        input_path = os.path.join(FOLDER_Z_FILMAMI, filename)
        output_path = os.path.join(FOLDER_Z_FILMAMI, filename.rsplit('.', 1)[0] + ".mp4")
        
        print(f"Kompresuję: {filename}...")
        
        try:
            (
                ffmpeg
                .input(input_path)
                .output(
                    output_path, 
                    vcodec='libx264', 
                    acodec='aac', 
                    pix_fmt='yuv420p', 
                    crf=28,
                    preset='slower', 
                    movflags='faststart'
                )
                .overwrite_output()
                .run(capture_stdout=True, capture_stderr=True)
            )
            
            if os.path.exists(output_path):
                os.remove(input_path) 
                print(f"✅ Skompresowano i usunięto: {filename}")
            
        except ffmpeg.Error as e:
            print(f"❌ Błąd w {filename}: {e.stderr.decode() if e.stderr else 'Nieznany błąd'}")

if __name__ == "__main__":
    convert_and_cleanup()
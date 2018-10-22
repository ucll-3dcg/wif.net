from struct import unpack
from PIL import Image, ImageTk
import sys
import tkinter as tk


def read_file(path):
    with open(path, 'rb') as file:
        def read_uint():
            return unpack('I', file.read(4))[0]

        def read_frame():
            width = read_uint()

            if width == 0:
                return None
            else:
                height = read_uint()
                buffer_size = 3 * width * height
                buffer = file.read(buffer_size)
                return Image.frombytes('RGB', (width, height), buffer)

        frames = []
        frame = read_frame()

        while frame:
            frames.append(frame)
            frame = read_frame()

        return frames


frames = read_file('test.wif')

class Application(tk.Frame):
    def __init__(self, master=None):
        super().__init__(master)
        self.frame_index = 0
        self.pack()
        self.create_widgets()
        self.tick()
        

    def create_widgets(self):
        self.photo = ImageTk.PhotoImage(frames[0])
        self.label = tk.Label(self, image=self.photo)
        self.label.pack()

    def tick(self):
        self.frame_index  = (self.frame_index + 1) % len(frames)
        self.photo = ImageTk.PhotoImage(frames[self.frame_index])
        self.label.configure(image=self.photo)
        self.after(33, self.tick)



root = tk.Tk()
app = Application(master=root)
app.mainloop()
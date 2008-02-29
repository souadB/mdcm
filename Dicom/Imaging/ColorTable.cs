using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Dicom.Imaging {
	public static class ColorTable {
		#region LUT
		public static Color[] Monochrome1 = InitGrayscaleLUT(true);
		public static Color[] Monochrome2 = InitGrayscaleLUT(false);

		private static Color[] InitGrayscaleLUT(bool reverse) {
			Color[] LUT = new Color[256];
			int i; byte b;
			if (reverse) {
				for (i = 0, b = 255; i < 256; i++, b--) {
					LUT[i] = Color.FromArgb(b, b, b);
				}
			} else {
				for (i = 0, b = 0; i < 256; i++, b++) {
					LUT[i] = Color.FromArgb(b, b, b);
				}
			}
			return LUT;
		}

		public static Color[] Reverse(Color[] lut) {
			Color[] clone = new Color[lut.Length];
			Array.Copy(lut, clone, clone.Length);
			Array.Reverse(clone);
			return clone;
		}

		public static Color[] LoadLUT(string file) {
			try {
				FileInfo fi = new FileInfo(file);
				if (fi.Length != (256 * 3)) return null;
				byte[] data = new byte[fi.Length];
				FileStream fs = fi.OpenRead();
				fs.Read(data, 0, (int)fi.Length);
				fs.Close();
				Color[] LUT = new Color[256];
				for (int i = 0; i < 256; i++) {
					LUT[i] = Color.FromArgb(data[i], data[i + 256], data[i + 512]);
				}
				return LUT;
			} catch {
				return null;
			}
		}

		public static void SaveLUT(string file, Color[] lut) {
			if (lut.Length != 256) return;
			FileStream fs = new FileStream(file, FileMode.Create);
			for (int i = 0; i < 256; i++) fs.WriteByte(lut[i].R);
			for (int i = 0; i < 256; i++) fs.WriteByte(lut[i].G);
			for (int i = 0; i < 256; i++) fs.WriteByte(lut[i].B);
			fs.Close();
		}

		public static void Apply(Image image, Color[] lut) {
			ColorPalette palette = image.Palette;
			for (int i = 0; i < palette.Entries.Length; i++)
				palette.Entries[i] = lut[i];
			image.Palette = palette;
		}
		#endregion
	}
}


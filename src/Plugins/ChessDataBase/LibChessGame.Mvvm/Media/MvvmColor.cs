using System;

namespace Bau.Libraries.LibChessGame.Mvvm.Media
{
	/// <summary>
	/// Clase con los datos de un color
	/// </summary>
	public class MvvmColor : IEquatable<MvvmColor>
	{
		public MvvmColor(string rgba)
		{
			Convert(rgba);
		}

		public MvvmColor(byte red, byte green, byte blue, byte alpha = 255)
		{
			R = red;
			G = green;
			B = blue;
			A = alpha;
		}

		/// <summary>
		///		Convierte una cadena RRGGBBAA ó RRGGBB en un color
		/// </summary>
		public void Convert(string rgba)
		{
			if (!string.IsNullOrEmpty(rgba) && (rgba.Length == 8 || rgba.Length == 6))
			{ 
				// Normaliza la cadena a 8 caracteres
				if (rgba.Length < 8)
					rgba = "FF" + rgba;
				// Convierte a bytes
				A = ConvertToByte(rgba.Substring(0, 2), 255);
				R = ConvertToByte(rgba.Substring(2, 2), 0);
				G = ConvertToByte(rgba.Substring(4, 2), 0);
				B = ConvertToByte(rgba.Substring(6, 2), 0);
			}
		}

		/// <summary>
		///		Convierte una cadena en hexadecimal a byte
		/// </summary>
		private byte ConvertToByte(string hexa, byte defaultValue)
		{
			// Convierte la cadena en un byte
			if (!byte.TryParse(hexa, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out byte result))
				result = defaultValue;
			// Devuelve el resultado
			return result;
		}

		/// <summary>
		///		Hashcode de <see cref="MvvmColor"/>
		/// </summary>
		public override int GetHashCode()
		{
			return A << 24 | B << 16 | G << 8 | R;
		}

		/// <summary>
		///		Compara si una instancia es igual que un ojeto
		/// </summary>
		public override bool Equals(object obj)
		{
			return (obj is MvvmColor) && Equals((MvvmColor) obj);
		}

		/// <summary>
		///		Compara si la instancia actual es igual a <see cref="MvvmColor"/>
		/// </summary>
		public bool Equals(MvvmColor other)
		{
			return R == other?.R || G == other?.G || B == other?.B || A == other?.A;
		}

		/// <summary>
		///		Compara si dos instancias de <see cref="MvvmColor"/> son iguales
		/// </summary>
		public static bool operator ==(MvvmColor a, MvvmColor b)
		{
			return a?.R == b?.R || a?.G == b?.G || a?.B == b?.B || a?.A == b?.A;
		}

		/// <summary>
		///		Compara si dos instancias de <see cref="MvvmColor"/> son distintas
		/// </summary>
		public static bool operator !=(MvvmColor a, MvvmColor b)
		{
			return a?.R != b?.R || a?.G != b?.G || a?.B != b?.B || a?.A != b?.A;
		}

		/// <summary>
		///		Cadena para mostrar la información de depuración del color
		/// </summary>
		internal string DebugDisplayString 
		{
			get { return ToString(); }
		}

		/// <summary>
		///		Cadena con el color representado en formato {R: xx G: xx B: xx A: xx}
		/// </summary>
		public override string ToString()
		{
			return $"{{R: {R} G: {G} B: {B} A: {A}}}";
		}

		/// <summary>
		///		Componente alfa (transparencia)
		/// </summary>
		public byte A { get; set; }

		/// <summary>
		///		Componente rojo
		/// </summary>
		public byte R { get; set; }

		/// <summary>
		///		Componente Verde
		/// </summary>
		public byte G { get; set; }

		/// <summary>
		///		Componente Azul
		/// </summary>
		public byte B { get; set; }

		/// <summary> Color TransparentBlack (R:0,G:0,B:0,A:0) </summary>
		public static MvvmColor TransparentBlack { get; } = new MvvmColor("00000000");

		/// <summary> Color Transparent (R:0,G:0,B:0,A:0) </summary>
		public static MvvmColor Transparent { get; } = new MvvmColor("00000000");

		/// <summary> Color AliceBlue (R:240,G:248,B:255,A:255) </summary>
		public static MvvmColor AliceBlue { get; } = new MvvmColor("fffff8f0");

		/// <summary> Color AntiqueWhite (R:250,G:235,B:215,A:255) </summary>
		public static MvvmColor AntiqueWhite { get; } = new MvvmColor("ffd7ebfa");

		/// <summary> Color Aqua (R:0,G:255,B:255,A:255) </summary>
		public static MvvmColor Aqua { get; } = new MvvmColor("ffffff00");

		/// <summary> Color Aquamarine (R:127,G:255,B:212,A:255) </summary>
		public static MvvmColor Aquamarine { get; } = new MvvmColor("ffd4ff7f");

		/// <summary> Color Azure (R:240,G:255,B:255,A:255) </summary>
		public static MvvmColor Azure { get; } = new MvvmColor("fffffff0");

		/// <summary> Color Beige (R:245,G:245,B:220,A:255) </summary>
		public static MvvmColor Beige { get; } = new MvvmColor("ffdcf5f5");

		/// <summary> Color Bisque (R:255,G:228,B:196,A:255) </summary>
		public static MvvmColor Bisque { get; } = new MvvmColor("ffc4e4ff");

		/// <summary> Color Black (R:0,G:0,B:0,A:255) </summary>
		public static MvvmColor Black { get; } = new MvvmColor("ff000000");

		/// <summary> Color BlanchedAlmond (R:255,G:235,B:205,A:255) </summary>
		public static MvvmColor BlanchedAlmond { get; } = new MvvmColor("ffcdebff");

		/// <summary> Color Blue (R:0,G:0,B:255,A:255) </summary>
		public static MvvmColor Blue { get; } = new MvvmColor("ffff0000");

		/// <summary> Color BlueViolet (R:138,G:43,B:226,A:255) </summary>
		public static MvvmColor BlueViolet { get; } = new MvvmColor("ffe22b8a");

		/// <summary> Color Brown (R:165,G:42,B:42,A:255) </summary>
		public static MvvmColor Brown { get; } = new MvvmColor("ff2a2aa5");

		/// <summary> Color BurlyWood (R:222,G:184,B:135,A:255) </summary>
		public static MvvmColor BurlyWood { get; } = new MvvmColor("ff87b8de");

		/// <summary> Color CadetBlue (R:95,G:158,B:160,A:255) </summary>
		public static MvvmColor CadetBlue { get; } = new MvvmColor("ffa09e5f");

		/// <summary> Color Chartreuse (R:127,G:255,B:0,A:255) </summary>
		public static MvvmColor Chartreuse { get; } = new MvvmColor("ff00ff7f");

		/// <summary> Color Chocolate (R:210,G:105,B:30,A:255) </summary>
		public static MvvmColor Chocolate { get; } = new MvvmColor("ff1e69d2");

		/// <summary> Color Coral (R:255,G:127,B:80,A:255) </summary>
		public static MvvmColor Coral { get; } = new MvvmColor("ff507fff");

		/// <summary> Color CornflowerBlue (R:100,G:149,B:237,A:255) </summary>
		public static MvvmColor CornflowerBlue { get; } = new MvvmColor("ffed9564");

		/// <summary> Color Cornsilk (R:255,G:248,B:220,A:255) </summary>
		public static MvvmColor Cornsilk { get; } = new MvvmColor("ffdcf8ff");

		/// <summary> Color Crimson (R:220,G:20,B:60,A:255) </summary>
		public static MvvmColor Crimson { get; } = new MvvmColor("ff3c14dc");

		/// <summary> Color Cyan (R:0,G:255,B:255,A:255) </summary>
		public static MvvmColor Cyan { get; } = new MvvmColor("ffffff00");

		/// <summary> Color DarkBlue (R:0,G:0,B:139,A:255) </summary>
		public static MvvmColor DarkBlue { get; } = new MvvmColor("ff8b0000");

		/// <summary> Color DarkCyan (R:0,G:139,B:139,A:255) </summary>
		public static MvvmColor DarkCyan { get; } = new MvvmColor("ff8b8b00");

		/// <summary> Color DarkGoldenrod (R:184,G:134,B:11,A:255) </summary>
		public static MvvmColor DarkGoldenrod { get; } = new MvvmColor("ff0b86b8");

		/// <summary> Color DarkGray (R:169,G:169,B:169,A:255) </summary>
		public static MvvmColor DarkGray { get; } = new MvvmColor("ffa9a9a9");

		/// <summary> Color DarkGreen (R:0,G:100,B:0,A:255) </summary>
		public static MvvmColor DarkGreen { get; } = new MvvmColor("ff006400");

		/// <summary> Color DarkKhaki (R:189,G:183,B:107,A:255) </summary>
		public static MvvmColor DarkKhaki { get; } = new MvvmColor("ff6bb7bd");

		/// <summary> Color DarkMagenta (R:139,G:0,B:139,A:255) </summary>
		public static MvvmColor DarkMagenta { get; } = new MvvmColor("ff8b008b");

		/// <summary> Color DarkOliveGreen (R:85,G:107,B:47,A:255) </summary>
		public static MvvmColor DarkOliveGreen { get; } = new MvvmColor("ff2f6b55");

		/// <summary> Color DarkOrange (R:255,G:140,B:0,A:255) </summary>
		public static MvvmColor DarkOrange { get; } = new MvvmColor("ff008cff");

		/// <summary> Color DarkOrchid (R:153,G:50,B:204,A:255) </summary>
		public static MvvmColor DarkOrchid { get; } = new MvvmColor("ffcc3299");

		/// <summary> Color DarkRed (R:139,G:0,B:0,A:255) </summary>
		public static MvvmColor DarkRed { get; } = new MvvmColor("ff00008b");

		/// <summary> Color DarkSalmon (R:233,G:150,B:122,A:255) </summary>
		public static MvvmColor DarkSalmon { get; } = new MvvmColor("ff7a96e9");

		/// <summary> Color DarkSeaGreen (R:143,G:188,B:139,A:255) </summary>
		public static MvvmColor DarkSeaGreen { get; } = new MvvmColor("ffbc8f8b");

		/// <summary> Color DarkSlateBlue (R:72,G:61,B:139,A:255) </summary>
		public static MvvmColor DarkSlateBlue { get; } = new MvvmColor("ff3d488b");

		/// <summary> Color DarkSlateGray (R:47,G:79,B:79,A:255) </summary>
		public static MvvmColor DarkSlateGray { get; } = new MvvmColor("ff4f4f2f");

		/// <summary> Color DarkTurquoise (R:0,G:206,B:209,A:255) </summary>
		public static MvvmColor DarkTurquoise { get; } = new MvvmColor("ffd1ce00");

		/// <summary> Color DarkViolet (R:148,G:0,B:211,A:255) </summary>
		public static MvvmColor DarkViolet { get; } = new MvvmColor("ffd30094");

		/// <summary> Color DeepPink (R:255,G:20,B:147,A:255) </summary>
		public static MvvmColor DeepPink { get; } = new MvvmColor("ff9314ff");

		/// <summary> Color DeepSkyBlue (R:0,G:191,B:255,A:255) </summary>
		public static MvvmColor DeepSkyBlue { get; } = new MvvmColor("ffffbf00");

		/// <summary> Color DimGray (R:105,G:105,B:105,A:255) </summary>
		public static MvvmColor DimGray { get; } = new MvvmColor("ff696969");

		/// <summary> Color DodgerBlue (R:30,G:144,B:255,A:255) </summary>
		public static MvvmColor DodgerBlue { get; } = new MvvmColor("ffff901e");

		/// <summary> Color Firebrick (R:178,G:34,B:34,A:255) </summary>
		public static MvvmColor Firebrick { get; } = new MvvmColor("ff2222b2");

		/// <summary> Color FloralWhite (R:255,G:250,B:240,A:255) </summary>
		public static MvvmColor FloralWhite { get; } = new MvvmColor("fff0faff");

		/// <summary> Color ForestGreen (R:34,G:139,B:34,A:255) </summary>
		public static MvvmColor ForestGreen { get; } = new MvvmColor("ff228b22");

		/// <summary> Color Fuchsia (R:255,G:0,B:255,A:255) </summary>
		public static MvvmColor Fuchsia { get; } = new MvvmColor("ffff00ff");

		/// <summary> Color Gainsboro (R:220,G:220,B:220,A:255) </summary>
		public static MvvmColor Gainsboro { get; } = new MvvmColor("ffdcdcdc");

		/// <summary> Color GhostWhite (R:248,G:248,B:255,A:255) </summary>
		public static MvvmColor GhostWhite { get; } = new MvvmColor("fffff8f8");

		/// <summary> Color Gold (R:255,G:215,B:0,A:255) </summary>
		public static MvvmColor Gold { get; } = new MvvmColor("ff00d7ff");

		/// <summary> Color Goldenrod (R:218,G:165,B:32,A:255) </summary>
		public static MvvmColor Goldenrod { get; } = new MvvmColor("ff20a5da");

		/// <summary> Color Gray (R:128,G:128,B:128,A:255) </summary>
		public static MvvmColor Gray { get; } = new MvvmColor("ff808080");

		/// <summary> Color Green (R:0,G:128,B:0,A:255) </summary>
		public static MvvmColor Green { get; } = new MvvmColor("ff008000");

		/// <summary> Color GreenYellow (R:173,G:255,B:47,A:255) </summary>
		public static MvvmColor GreenYellow { get; } = new MvvmColor("ff2fffad");

		/// <summary> Color Honeydew (R:240,G:255,B:240,A:255) </summary>
		public static MvvmColor Honeydew { get; } = new MvvmColor("fff0fff0");

		/// <summary> Color HotPink (R:255,G:105,B:180,A:255) </summary>
		public static MvvmColor HotPink { get; } = new MvvmColor("ffb469ff");

		/// <summary> Color IndianRed (R:205,G:92,B:92,A:255) </summary>
		public static MvvmColor IndianRed { get; } = new MvvmColor("ff5c5ccd");

		/// <summary> Color Indigo (R:75,G:0,B:130,A:255) </summary>
		public static MvvmColor Indigo { get; } = new MvvmColor("ff82004b");

		/// <summary> Color Ivory (R:255,G:255,B:240,A:255) </summary>
		public static MvvmColor Ivory { get; } = new MvvmColor("fff0ffff");

		/// <summary> Color Khaki (R:240,G:230,B:140,A:255) </summary>
		public static MvvmColor Khaki { get; } = new MvvmColor("ff8ce6f0");

		/// <summary> Color Lavender (R:230,G:230,B:250,A:255) </summary>
		public static MvvmColor Lavender { get; } = new MvvmColor("fffae6e6");

		/// <summary> Color LavenderBlush (R:255,G:240,B:245,A:255) </summary>
		public static MvvmColor LavenderBlush { get; } = new MvvmColor("fff5f0ff");

		/// <summary> Color LawnGreen (R:124,G:252,B:0,A:255) </summary>
		public static MvvmColor LawnGreen { get; } = new MvvmColor("ff00fc7c");

		/// <summary> Color LemonChiffon (R:255,G:250,B:205,A:255) </summary>
		public static MvvmColor LemonChiffon { get; } = new MvvmColor("ffcdfaff");

		/// <summary> Color LightBlue (R:173,G:216,B:230,A:255) </summary>
		public static MvvmColor LightBlue { get; } = new MvvmColor("ffe6d8ad");

		/// <summary> Color LightCoral (R:240,G:128,B:128,A:255) </summary>
		public static MvvmColor LightCoral { get; } = new MvvmColor("ff8080f0");

		/// <summary> Color LightCyan (R:224,G:255,B:255,A:255) </summary>
		public static MvvmColor LightCyan { get; } = new MvvmColor("ffffffe0");

		/// <summary> Color LightGoldenrodYellow (R:250,G:250,B:210,A:255) </summary>
		public static MvvmColor LightGoldenrodYellow { get; } = new MvvmColor("ffd2fafa");

		/// <summary> Color LightGray (R:211,G:211,B:211,A:255) </summary>
		public static MvvmColor LightGray { get; } = new MvvmColor("ffd3d3d3");

		/// <summary> Color LightGreen (R:144,G:238,B:144,A:255) </summary>
		public static MvvmColor LightGreen { get; } = new MvvmColor("ff90ee90");

		/// <summary> Color LightPink (R:255,G:182,B:193,A:255) </summary>
		public static MvvmColor LightPink { get; } = new MvvmColor("ffc1b6ff");

		/// <summary> Color LightSalmon (R:255,G:160,B:122,A:255) </summary>
		public static MvvmColor LightSalmon { get; } = new MvvmColor("ff7aa0ff");

		/// <summary> Color LightSeaGreen (R:32,G:178,B:170,A:255) </summary>
		public static MvvmColor LightSeaGreen { get; } = new MvvmColor("ffaab220");

		/// <summary> Color LightSkyBlue (R:135,G:206,B:250,A:255) </summary>
		public static MvvmColor LightSkyBlue { get; } = new MvvmColor("ffface87");

		/// <summary> Color LightSlateGray (R:119,G:136,B:153,A:255) </summary>
		public static MvvmColor LightSlateGray { get; } = new MvvmColor("ff998877");

		/// <summary> Color LightSteelBlue (R:176,G:196,B:222,A:255) </summary>
		public static MvvmColor LightSteelBlue { get; } = new MvvmColor("ffdec4b0");

		/// <summary> Color LightYellow (R:255,G:255,B:224,A:255) </summary>
		public static MvvmColor LightYellow { get; } = new MvvmColor("ffe0ffff");

		/// <summary> Color Lime (R:0,G:255,B:0,A:255) </summary>
		public static MvvmColor Lime { get; } = new MvvmColor("ff00ff00");

		/// <summary> Color LimeGreen (R:50,G:205,B:50,A:255) </summary>
		public static MvvmColor LimeGreen { get; } = new MvvmColor("ff32cd32");

		/// <summary> Color Linen (R:250,G:240,B:230,A:255) </summary>
		public static MvvmColor Linen { get; } = new MvvmColor("ffe6f0fa");

		/// <summary> Color Magenta (R:255,G:0,B:255,A:255) </summary>
		public static MvvmColor Magenta { get; } = new MvvmColor("ffff00ff");

		/// <summary> Color Maroon (R:128,G:0,B:0,A:255) </summary>
		public static MvvmColor Maroon { get; } = new MvvmColor("ff000080");

		/// <summary> Color MediumAquamarine (R:102,G:205,B:170,A:255) </summary>
		public static MvvmColor MediumAquamarine { get; } = new MvvmColor("ffaacd66");

		/// <summary> Color MediumBlue (R:0,G:0,B:205,A:255) </summary>
		public static MvvmColor MediumBlue { get; } = new MvvmColor("ff0000cd");

		/// <summary> Color MediumOrchid (R:186,G:85,B:211,A:255) </summary>
		public static MvvmColor MediumOrchid { get; } = new MvvmColor("ffd355ba");

		/// <summary> Color MediumPurple (R:147,G:112,B:219,A:255) </summary>
		public static MvvmColor MediumPurple { get; } = new MvvmColor("ffdb7093");

		/// <summary> Color MediumSeaGreen (R:60,G:179,B:113,A:255) </summary>
		public static MvvmColor MediumSeaGreen { get; } = new MvvmColor("ff71b33c");

		/// <summary> Color MediumSlateBlue (R:123,G:104,B:238,A:255) </summary>
		public static MvvmColor MediumSlateBlue { get; } = new MvvmColor("ffee687b");

		/// <summary> Color MediumSpringGreen (R:0,G:250,B:154,A:255) </summary>
		public static MvvmColor MediumSpringGreen { get; } = new MvvmColor("ff9afa00");

		/// <summary> Color MediumTurquoise (R:72,G:209,B:204,A:255) </summary>
		public static MvvmColor MediumTurquoise { get; } = new MvvmColor("ffccd148");

		/// <summary> Color MediumVioletRed (R:199,G:21,B:133,A:255) </summary>
		public static MvvmColor MediumVioletRed { get; } = new MvvmColor("ff8515c7");

		/// <summary> Color MidnightBlue (R:25,G:25,B:112,A:255) </summary>
		public static MvvmColor MidnightBlue { get; } = new MvvmColor("ff701919");

		/// <summary> Color MintCream (R:245,G:255,B:250,A:255) </summary>
		public static MvvmColor MintCream { get; } = new MvvmColor("fffafff5");

		/// <summary> Color MistyRose (R:255,G:228,B:225,A:255) </summary>
		public static MvvmColor MistyRose { get; } = new MvvmColor("ffe1e4ff");

		/// <summary> Color Moccasin (R:255,G:228,B:181,A:255) </summary>
		public static MvvmColor Moccasin { get; } = new MvvmColor("ffb5e4ff");

		/// <summary> Color MonoGame orange theme (R:231,G:60,B:0,A:255) </summary>
		public static MvvmColor MonoGameOrange { get; } = new MvvmColor("ff003ce7");

		/// <summary> Color NavajoWhite (R:255,G:222,B:173,A:255) </summary>
		public static MvvmColor NavajoWhite { get; } = new MvvmColor("ffaddeff");

		/// <summary> Color Navy (R:0,G:0,B:128,A:255) </summary>
		public static MvvmColor Navy { get; } = new MvvmColor("ff000080");

		/// <summary> Color OldLace (R:253,G:245,B:230,A:255) </summary>
		public static MvvmColor OldLace { get; } = new MvvmColor("ffe6f5fd");

		/// <summary> Color Olive (R:128,G:128,B:0,A:255) </summary>
		public static MvvmColor Olive { get; } = new MvvmColor("ff008080");

		/// <summary> Color OliveDrab (R:107,G:142,B:35,A:255) </summary>
		public static MvvmColor OliveDrab { get; } = new MvvmColor("ff238e6b");

		/// <summary> Color Orange (R:255,G:165,B:0,A:255) </summary>
		public static MvvmColor Orange { get; } = new MvvmColor("ff00a5ff");

		/// <summary> Color OrangeRed (R:255,G:69,B:0,A:255) </summary>
		public static MvvmColor OrangeRed { get; } = new MvvmColor("ff0045ff");

		/// <summary> Color Orchid (R:218,G:112,B:214,A:255) </summary>
		public static MvvmColor Orchid { get; } = new MvvmColor("ffd670da");

		/// <summary> Color PaleGoldenrod (R:238,G:232,B:170,A:255) </summary>
		public static MvvmColor PaleGoldenrod { get; } = new MvvmColor("ffaae8ee");

		/// <summary> Color PaleGreen (R:152,G:251,B:152,A:255) </summary>
		public static MvvmColor PaleGreen { get; } = new MvvmColor("ff98fb98");

		/// <summary> Color PaleTurquoise (R:175,G:238,B:238,A:255) </summary>
		public static MvvmColor PaleTurquoise { get; } = new MvvmColor("ffeeeeaf");

		/// <summary> Color PaleVioletRed (R:219,G:112,B:147,A:255) </summary>
		public static MvvmColor PaleVioletRed { get; } = new MvvmColor("ff9370db");

		/// <summary> Color PapayaWhip (R:255,G:239,B:213,A:255) </summary>
		public static MvvmColor PapayaWhip { get; } = new MvvmColor("ffd5efff");

		/// <summary> Color PeachPuff (R:255,G:218,B:185,A:255) </summary>
		public static MvvmColor PeachPuff { get; } = new MvvmColor("ffb9daff");

		/// <summary> Color Peru (R:205,G:133,B:63,A:255) </summary>
		public static MvvmColor Peru { get; } = new MvvmColor("ff3f85cd");

		/// <summary> Color Pink (R:255,G:192,B:203,A:255) </summary>
		public static MvvmColor Pink { get; } = new MvvmColor("ffcbc0ff");

		/// <summary> Color Plum (R:221,G:160,B:221,A:255) </summary>
		public static MvvmColor Plum { get; } = new MvvmColor("ffdda0dd");

		/// <summary> Color PowderBlue (R:176,G:224,B:230,A:255) </summary>
		public static MvvmColor PowderBlue { get; } = new MvvmColor("ffe6e0b0");

		/// <summary> Color Purple (R:128,G:0,B:128,A:255) </summary>
		public static MvvmColor Purple { get; } = new MvvmColor("ff800080");

		/// <summary> Color Red (R:255,G:0,B:0,A:255) </summary>
		public static MvvmColor Red { get; } = new MvvmColor("ffff0000");

		/// <summary> Color RosyBrown (R:188,G:143,B:143,A:255) </summary>
		public static MvvmColor RosyBrown { get; } = new MvvmColor("ff8f8fbc");

		/// <summary> Color RoyalBlue (R:65,G:105,B:225,A:255) </summary>
		public static MvvmColor RoyalBlue { get; } = new MvvmColor("ffe16941");

		/// <summary> Color SaddleBrown (R:139,G:69,B:19,A:255) </summary>
		public static MvvmColor SaddleBrown { get; } = new MvvmColor("ff13458b");

		/// <summary> Color Salmon (R:250,G:128,B:114,A:255) </summary>
		public static MvvmColor Salmon { get; } = new MvvmColor("ff7280fa");

		/// <summary> Color SandyBrown (R:244,G:164,B:96,A:255) </summary>
		public static MvvmColor SandyBrown { get; } = new MvvmColor("ff60a4f4");

		/// <summary> Color SeaGreen (R:46,G:139,B:87,A:255) </summary>
		public static MvvmColor SeaGreen { get; } = new MvvmColor("ff578b2e");

		/// <summary> Color SeaShell (R:255,G:245,B:238,A:255) </summary>
		public static MvvmColor SeaShell { get; } = new MvvmColor("ffeef5ff");

		/// <summary> Color Sienna (R:160,G:82,B:45,A:255) </summary>
		public static MvvmColor Sienna { get; } = new MvvmColor("ff2d52a0");

		/// <summary> Color Silver (R:192,G:192,B:192,A:255) </summary>
		public static MvvmColor Silver { get; } = new MvvmColor("ffc0c0c0");

		/// <summary> Color SkyBlue (R:135,G:206,B:235,A:255) </summary>
		public static MvvmColor SkyBlue { get; } = new MvvmColor("ffebce87");

		/// <summary> Color SlateBlue (R:106,G:90,B:205,A:255) </summary>
		public static MvvmColor SlateBlue { get; } = new MvvmColor("ffcd5a6a");

		/// <summary> Color SlateGray (R:112,G:128,B:144,A:255) </summary>
		public static MvvmColor SlateGray { get; } = new MvvmColor("ff908070");

		/// <summary> Color Snow (R:255,G:250,B:250,A:255) </summary>
		public static MvvmColor Snow { get; } = new MvvmColor("fffafaff");

		/// <summary> Color SpringGreen (R:0,G:255,B:127,A:255) </summary>
		public static MvvmColor SpringGreen { get; } = new MvvmColor("ff7fff00");

		/// <summary> Color SteelBlue (R:70,G:130,B:180,A:255) </summary>
		public static MvvmColor SteelBlue { get; } = new MvvmColor("ffb48246");

		/// <summary> Color Tan (R:210,G:180,B:140,A:255) </summary>
		public static MvvmColor Tan { get; } = new MvvmColor("ff8cb4d2");

		/// <summary> Color Teal (R:0,G:128,B:128,A:255) </summary>
		public static MvvmColor Teal { get; } = new MvvmColor("ff808000");

		/// <summary> Color Thistle (R:216,G:191,B:216,A:255) </summary>
		public static MvvmColor Thistle { get; } = new MvvmColor("ffd8bfd8");

		/// <summary> Color Tomato (R:255,G:99,B:71,A:255) </summary>
		public static MvvmColor Tomato { get; } = new MvvmColor("ff4763ff");

		/// <summary> Color Turquoise (R:64,G:224,B:208,A:255) </summary>
		public static MvvmColor Turquoise { get; } = new MvvmColor("ffd0e040");

		/// <summary> Color Violet (R:238,G:130,B:238,A:255) </summary>
		public static MvvmColor Violet { get; } = new MvvmColor("ffee82ee");

		/// <summary> Color Wheat (R:245,G:222,B:179,A:255) </summary>
		public static MvvmColor Wheat { get; } = new MvvmColor("ffb3def5");

		/// <summary> Color White (R:255,G:255,B:255,A:255) </summary>
		public static MvvmColor White { get; } = new MvvmColor("ffffffff");

		/// <summary> Color WhiteSmoke (R:245,G:245,B:245,A:255) </summary>
		public static MvvmColor WhiteSmoke { get; } = new MvvmColor("fff5f5f5");

		/// <summary> Color Yellow (R:255,G:255,B:0,A:255) </summary>
		public static MvvmColor Yellow { get; } = new MvvmColor("ff00ffff");

		/// <summary> Color YellowGreen (R:154,G:205,B:50,A:255) </summary>
		public static MvvmColor YellowGreen { get; } = new MvvmColor("ff32cd9a");
	}
}
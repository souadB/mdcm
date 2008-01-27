﻿// mDCM: A C# DICOM library
//
// Copyright (c) 2006-2008  Colby Dillion
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Author:
//    Colby Dillion (colby.dillion@gmail.com)

#if DOTNET35

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Xml.Linq;

using Dicom.Utility;

public static class ArrayHelper {
	public static Array Shuffle(this Array array) {
		Random rand = new Random();
		int n = array.Length;
		while (--n > 0) {
			int k = rand.Next(n + 1);
			array.Swap(n, k);
		}
		return array;
	}

	public static void Swap(this Array array, int source, int destination) {
		object obj = array.GetValue(source);
		array.SetValue(array.GetValue(destination), source);
		array.SetValue(obj, destination);
	}
}

public static class StringHelper {
	public static bool Match(this string str, string pattern) {
		return Wildcard.Match(pattern, str);
	}

	public static bool Match(this string str, string pattern, bool caseSensitive) {
		return Wildcard.Match(pattern, str, caseSensitive);
	}

	public static string MD5(this string str) {
		MD5 md5 = new MD5CryptoServiceProvider();
		byte[] bytes = ASCIIEncoding.Default.GetBytes(str);
		bytes = md5.ComputeHash(bytes);
		return BitConverter.ToString(bytes);
	}

	public static string SHA1(this string str) {
		SHA1 sha1 = new SHA1CryptoServiceProvider();
		byte[] bytes = ASCIIEncoding.Default.GetBytes(str);
		bytes = sha1.ComputeHash(bytes);
		return BitConverter.ToString(bytes);
	}
}

public static class XmlHelper {
	public static string FirstText(this XElement element) {
		foreach (XNode node in element.Nodes()) {
			if (node is XText) {
				XText text = (XText)node;
				return text.Value;
			}
		}
		return String.Empty;
	}
}

#endif
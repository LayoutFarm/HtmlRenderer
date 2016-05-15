// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace Icu
{
	public class Locale 
	{
		/// <summary>
		/// Construct a default locale object, a Locale for the default locale ID
		/// </summary>
		public Locale()
		{
			Id = Canonicalize(CultureInfo.CurrentUICulture.Name);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Icu.Locale"/> class.
		/// <paramref name="localeId"/> can either be a full locale like "en-US", or a language.
		/// </summary>
		/// <param name="localeId">Locale.</param>
		public Locale(string localeId)
		{
			Id = Canonicalize(localeId);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Icu.Locale"/> class.
		/// </summary>
		/// <param name="language">Language.</param>
		/// <param name="country">Country or script.</param>
		public Locale(string language, string country):
			this (language, country, null, null)
		{
		}

		public Locale(string language, string country, string variant):
			this (language, country, variant, null)
		{
		}

		public Locale(string language, string country, string variant, string keywordsAndValues)
		{
			var bldr = new StringBuilder();
			bldr.AppendFormat("{0}_{1}", language, country);
			if (!string.IsNullOrEmpty(variant))
			{
				bldr.AppendFormat("_{0}", variant);
			}
			if (!string.IsNullOrEmpty(keywordsAndValues))
			{
				bldr.AppendFormat("@{0}", keywordsAndValues);
			}
			Id = Canonicalize(bldr.ToString());
		}

		public object Clone()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets or sets the identifier of the ICU locale.
		/// </summary>
		public string Id { get; private set; }

		/// <summary>
		/// Gets the two-letter language code as defined by ISO-639.
		/// </summary>
		public string Language
		{
			get { return GetString(NativeMethods.uloc_getLanguage, Id); }
		}

		/// <summary>
		/// Gets the script code as defined by ISO-15924.
		/// </summary>
		public string Script
		{
			get { return GetString(NativeMethods.uloc_getScript, Id); }
		}

		/// <summary>
		/// Gets the two-letter country code as defined by ISO-3166.
		/// </summary>
		public string Country
		{
			get { return GetString(NativeMethods.uloc_getCountry, Id); }
		}

		public string Variant
		{
			get { return GetString(NativeMethods.uloc_getVariant, Id); }
		}

		public string Name
		{
			get { return GetString(NativeMethods.uloc_getName, Id); }
		}

		public string BaseName
		{
			get { return GetString(NativeMethods.uloc_getBaseName, Id); }
		}

		/// <summary>
		/// Gets the 3-letter ISO 639-3 language code
		/// </summary>
		public string Iso3Language
		{
			get { return Marshal.PtrToStringAnsi(NativeMethods.uloc_getISO3Language(Id)); }
		}

		/// <summary>
		/// Gets the 3-letter ISO country code
		/// </summary>
		public string Iso3Country
		{
			get { return Marshal.PtrToStringAnsi(NativeMethods.uloc_getISO3Country(Id)); }
		}

		public int Lcid
		{
			get { return NativeMethods.uloc_getLCID(Id); }
		}

		public string DisplayLanguage
		{
			get { return GetDisplayLanguage(new Locale().Id); }
		}

		public string GetDisplayLanguage(Locale displayLocale)
		{
			return GetString(NativeMethods.uloc_getDisplayLanguage, Id, displayLocale.Id);
		}

		public string DisplayCountry
		{
			get { return GetDisplayCountry(new Locale().Id); }
		}

		public string GetDisplayCountry(Locale displayLocale)
		{
			return GetString(NativeMethods.uloc_getDisplayCountry, Id, displayLocale.Id);
		}

		public string DisplayName
		{
			get { return GetDisplayName(new Locale().Id); }
		}

		public string GetDisplayName(Locale displayLocale)
		{
			return GetString(NativeMethods.uloc_getDisplayName, Id, displayLocale.Id);
		}

		public override string ToString()
		{
			return Id;
		}

		public static List<Locale> AvailableLocales
		{
			get
			{
				var locales = new List<Locale>();
				for (int i = 0; i < NativeMethods.uloc_countAvailable(); i++)
					locales.Add(new Locale(Marshal.PtrToStringAnsi(NativeMethods.uloc_getAvailable(i))));
				return locales;
			}
		}

		public static implicit operator Locale(string localeId)
		{
			return new Locale(localeId);
		}

		#region ICU wrapper methods

		private delegate int GetStringMethod(string localeID, IntPtr name,
			int nameCapacity, out ErrorCode err);
		private delegate int GetStringDisplayMethod(string localeID, string displayLocaleID, IntPtr name,
			int nameCapacity, out ErrorCode err);

		private static string GetString(GetStringMethod method, string localeId)
		{
			const int nSize = 255;
			IntPtr resPtr = Marshal.AllocCoTaskMem(nSize);
			try
			{
				ErrorCode err;
				method(localeId, resPtr, nSize, out err);
				if (!Failure(err))
					return Marshal.PtrToStringAnsi(resPtr);
				return string.Empty;
			}
			finally
			{
				Marshal.FreeCoTaskMem(resPtr);
			}
		}

		private static string GetString(GetStringDisplayMethod method, string localeId, string displayLocaleId)
		{
			const int nSize = 255;
			IntPtr resPtr = Marshal.AllocCoTaskMem(nSize * 2);
			try
			{
				ErrorCode err;
				method(localeId, displayLocaleId, resPtr, nSize, out err);
				if (!Failure(err))
					return Marshal.PtrToStringUni(resPtr);
				return string.Empty;
			}
			finally
			{
				Marshal.FreeCoTaskMem(resPtr);
			}
		}

		/// <summary>
		/// Gets the full name for the specified locale.
		/// </summary>
		private static string Canonicalize(string localeID)
		{
			return GetString(NativeMethods.uloc_canonicalize, localeID);
		}

		private static bool Failure(ErrorCode err)
		{
			return (int)err > (int)ErrorCode.ZERO_ERROR;
		}
		#endregion
	}
}

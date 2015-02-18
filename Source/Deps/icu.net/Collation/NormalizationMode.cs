// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
namespace Icu.Collation
{
	/// <summary>
	/// Controls whether the normalization check and necessary normalizations
	/// are performed.
	/// </summary>
	public enum NormalizationMode
	{
		Default = -1,
		/// <summary>
		/// no normalization check is performed. The correctness of the result is guaranteed only if the
		/// input data is in so-called FCD form (see users manual for more info).
		/// </summary>
		Off = 16,
		/// <summary>
		/// an incremental check is performed to see whether the input data is in the FCD form.
		/// If the data is not in the FCD form, incremental NFD normalization is performed
		/// </summary>
		On = 17
	}
}
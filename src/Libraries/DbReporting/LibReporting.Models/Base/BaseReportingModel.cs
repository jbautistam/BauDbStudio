using System;

namespace Bau.Libraries.LibReporting.Models.Base
{
	/// <summary>
	///		Base para los modelos de reporting
	/// </summary>
	public class BaseReportingModel
	{
		/// <summary>
		///		Id global del elemento
		/// </summary>
		public string Id { get; set; } = Guid.NewGuid().ToString();
	}
}

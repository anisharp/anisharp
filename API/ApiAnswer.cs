using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniSharp.API
{

    /// <summary>
    /// Encapsulats an API-Answer
    /// </summary>
    abstract class ApiAnswer
    {
	
	private ReturnCode code;
	public ReturnCode Code {
		get
		{
			return code;
		}
	}


	public String GetMessage()
	{
		return code.ToString("F");
	}


	public ApiAnswer(int code)
	{
		try {
			this.code = (ReturnCode) code;
		} catch (InvalidCastException) {
			throw new ArgumentException();
		}
	}
    }
}

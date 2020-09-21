using System.Collections;
using System.Collections.Generic;

public static class CSharpExtensions
{
    public static bool Contains(this string[] selfStringArray, string value)
	{
		foreach(var item in selfStringArray) {
			if(item == value) {
				return true;
			}
		}
		return false;
	}
}

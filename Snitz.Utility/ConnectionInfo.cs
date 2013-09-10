using System;
using System.Text;

namespace Snitz.Utility {
	/// <summary>
	/// Helper class to encrypt and decrypt connection string info
	/// </summary>
	public class ConnectionInfo {

		public static string DecryptDBConnectionString(string InputConnectionString){

			// If the variable is blank, return the input
			if(InputConnectionString.Equals(string.Empty)){
				return InputConnectionString;
			}

			// Create an instance of the encryption API
			// We assume the key has been encrypted on this machine and not by a user
			DataProtector dp = new DataProtector(Store.Machine);

			// Use the API to decrypt the connection string
			// API works with bytes so we need to convert to and from byte arrays
			byte[] decryptedData = dp.Decrypt( Convert.FromBase64String( InputConnectionString ), null );
			
			// Return the decyrpted data to the string
			return Encoding.ASCII.GetString( decryptedData );
		}

		public static string EncryptDBConnectionString(string encryptedString){

			// Create an instance of the encryption API
			// We assume the key has been encrypted on this machine and not by a user
			DataProtector dp = new DataProtector(Store.Machine);

			// Use the API to encrypt the connection string
			// API works with bytes so we need to convert to and from byte arrays
			byte[] dataBytes = Encoding.ASCII.GetBytes( encryptedString );
			byte[] encryptedBytes = dp.Encrypt( dataBytes, null );

			// Return the encyrpted data to the string
			return Convert.ToBase64String( encryptedBytes );
		}
	}
}

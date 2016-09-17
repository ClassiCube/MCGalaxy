using System;
using System.Collections.Generic;

namespace Sharkbite.Irc
{
	/// <summary>
	/// Encapsulates the collection of properties sent by the IRC server
	/// after registration.
	/// </summary>
	/// <remarks>See the server_properties.pdf file for a list of comon properties.</remarks>
	/// <example><code>
	/// //This will only be non null if the connection has already received
	/// //a '005' reply and that such a reply is actually sent by the server.
	/// //This will happen right after registration.
	/// //Instances are only retrieved from a Connection and not instantiated directly.
	/// ServerProperties properties = connection.ServerProperties;
	/// //It should always be tested for null
	/// if( properties != null ) {
	/// Console.Writeline("NICKLEN is" + properties["NICKLEN"] );
	/// }
	/// //Only a handful of properties will ever be available.
	/// </code></example>
	public sealed class ServerProperties
	{
		private Dictionary<string, string> properties;

		/// <summary>
		/// Instances should only be created by the Connection class.
		/// </summary>
		internal ServerProperties()
		{
			properties = new Dictionary<string, string>();
		}

		/// <summary>
		/// Read-only indexer for the various server
		/// property strings.
		/// </summary>
		/// <returns>The string sent by the server or <see cref="String.Empty"/> if not present.</returns>
		public string this [ string key ] 
		{
			get
			{
				string value;
				properties.TryGetValue( key, out value );
				return value ?? "";
			}
		}

		/// <summary>
		/// Add a property retrieved from the IRC 
		/// server.
		/// </summary>
		internal void SetProperty( string key, string propertyValue )
		{
			properties[key] = propertyValue;
		}

		/// <summary>
		/// Get a read-only enumeration of all the elements
		/// in this object.
		/// </summary>
		/// <returns>An IDictionaryEnumerator type enumeration.</returns>
		/// <example><code>
		/// //To loop over all the values	
		/// foreach( DictionaryEntry entry in connection.ServerProperties ) 
		/// {
		/// Console.WriteLine("Key:" + entry.Key + " Value:" + entry.Value );
		/// }
		/// </code></example>
		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			return properties.GetEnumerator();
		}
		/// <summary>
		/// Test if this instance contains a given key.
		/// </summary>
		/// <param name="key">The server properties key to test.</param>
		/// <returns>True if it is present.</returns>
		public bool ContainsKey( string key ) 
		{
			return properties.ContainsKey( key );
		}

	}
}

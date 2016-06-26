/*
	Copyright 2012 MCForge
 
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.opensource.org/licenses/ecl2.php
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
*/
namespace MCGalaxy
{
	public interface IBeat
	{
		/// <summary> Gets or sets the URL. </summary>
		/// <value> The URL. </value>
		string URL {get; }

		/// <summary> Prepares this instance. </summary>
		/// <returns></returns>
		string Prepare();

		/// <summary> Gets a value indicating whether this <see cref="IBeat"/> is persistance. </summary>
		/// <value> <c>true</c> if persistance; otherwise, <c>false</c>. </value>
		bool Persistance {get; }

		/// <summary> Called when a response is recieved. </summary>
		/// <param name="resonse">The resonse.</param>
		void OnResponse(string resonse);
	}
}

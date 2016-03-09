/*
	Copyright 2011 MCForge
		
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
namespace MCGalaxy.Utils.Crypto {
    /// <summary>
    /// Interface for Simple Crypto Service
    /// </summary>
    public interface ICryptoService {
        /// <summary>
        /// Gets or sets the number of iterations the hash will go through
        /// </summary>
        int HashIterations { get; set; }

        /// <summary>
        /// Gets or sets the size of salt that will be generated if no Salt was set
        /// </summary>
        int SaltSize { get; set; }

        /// <summary>
        /// Gets or sets the plain text to be hashed
        /// </summary>
        string PlainText { get; set; }

        /// <summary>
        /// Gets the base 64 encoded string of the hashed PlainText
        /// </summary>
        string HashedText { get; }

        /// <summary>
        /// Gets or sets the salt that will be used in computing the HashedText
        /// </summary>
        string Salt { get; }

        /// <summary>
        /// Compute the hash
        /// </summary>
        /// <returns>the computed hash: HashedText</returns>
        string Compute();

        /// <summary>
        /// Compute the hash using default generated salt
        /// </summary>
        /// <param name="textToHash"></param>
        /// <returns></returns>
        string Compute(string textToHash);

        /// <summary>
        /// Compute the hash that will also generate a salt from parameters
        /// </summary>
        /// <param name="textToHash">The text to be hashed</param>
        /// <param name="saltSize">The size of the salt to be generated</param>
        /// <param name="hashIterations"></param>
        /// <returns>the computed hash: HashedText</returns>
        string Compute(string textToHash, int saltSize, int hashIterations);

        /// <summary>
        /// Compute the hash that will utilize the passed salt
        /// </summary>
        /// <param name="textToHash">The text to be hashed</param>
        /// <param name="salt">The salt to be used in the computation</param>
        /// <returns>the computed hash: HashedText</returns>
        string Compute(string textToHash, string salt);

        /// <summary>
        /// Get the time in milliseconds it takes to complete the hash for the iterations
        /// </summary>
        /// <param name="iteration"></param>
        /// <returns></returns>
        int GetElapsedTimeForIteration(int iteration);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    internal static class Tools
    {
        public static bool TryGetCoordinates(string address, out (double Latitude, double Longitude) coordinates)
        {
            coordinates = (0, 0); // Initialisation par défaut

            if (string.IsNullOrWhiteSpace(address))
                return false;

            // Données simulées pour les tests
            var simulatedResults = new Dictionary<string, (double Latitude, double Longitude)>
    {
        { "123 Main St", (40.7128, -74.0060) }, // Exemple pour NYC
        { "456 Broadway", (34.0522, -118.2437) } // Exemple pour LA
    };

            // Rechercher l'adresse dans les données simulées
            return simulatedResults.TryGetValue(address, out coordinates);
        }

    }
}

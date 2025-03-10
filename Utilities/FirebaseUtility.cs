using FireSharp.Config;
using FireSharp.Interfaces;

namespace WebPortal.Utilities
{
    public class FirebaseUtility
    {
        private readonly IFirebaseClient _client;

        public FirebaseUtility()
        {
            var config = new FirebaseConfig
            {
                AuthSecret = "766ddgh23KRAO45YBgSccV8sdtMOviH6FDEqH2EE",
                BasePath = "https://worldgate-43164-default-rtdb.firebaseio.com/"


                //AuthSecret = "Y4tk9O3wwfAgOdAG2497pYO82N5rev7A9xjFeVjQ",
                //BasePath = "https://fir-5e7cc-default-rtdb.firebaseio.com"
            };

            _client = new FireSharp.FirebaseClient(config);
        }

        public IFirebaseClient GetClient()
        {
            return _client;
        }
    }
}

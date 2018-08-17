using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Timers;

namespace PiUpdate
{

    class Program
    {
        static ElectricBox box = new ElectricBox();
        static string sceneId = "31003dd2-083e-4c90-8cde-e8e80eac969b";
        static string guid;

        static void Main(string[] args)
        {
            Timer aTimer = new Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimerTick);
            aTimer.Interval = 10000;
            aTimer.Enabled = true;
            aTimer.Start();
            //
            guid = GetGUIDBySceneIDFromVertxAsync();
            Console.WriteLine("MAIN : " + guid);

            box.Add(new Component("KEY_ANIMATION"));
            box.Add(new Component("SWITCH_ONE"));
            box.Add(new Component("SWITCH_TWO"));
            box.Add(new Component("SWITCH_THREE"));
            box.Add(new Component("DOOR_ANIMATION"));
            box.Add(new Component("FUSE_ANIMATION"));

            WebClient client = new WebClient();
            client.BaseAddress = "https://staging.vertx.cloud";
            client.Headers.Add("Content-Type", "application/json");

            Console.WriteLine("Program ready!\n");

            //Constant service running to check state change
            while (true)
            {
                KeyboardUpdate();

                foreach (Component component in box.getComponents())
                {
                   
                    bool changed = component.isChanged();

                    if (changed)
                    {
                        Console.WriteLine(guid);

                        try
                        {
                            string json = component.getJson();
                            client.Headers.Add("Content-Type", "application/json");
                            Console.WriteLine(json);
                            Console.WriteLine("Client sending to VERTX");
                            client.UploadData("/session/fire/" + sceneId + "/" + guid + "/OnUpdate", System.Text.UTF8Encoding.UTF8.GetBytes(json));
                            Console.WriteLine("Data sent");
                        }
                        catch (WebException webEcxeption)
                        {
                            Console.WriteLine("WebException thrown => " + webEcxeption.Message);
                            //guid = GetGUIDBySceneIDFromVertx();
                        }
                    }//end if
                }//end foreach
            }//end while
        }//end main

        private static void KeyboardUpdate()
        {
            var input = Console.ReadKey();

            // Update all values (Leave this alone)
            foreach (var component in box.getComponents())
                component.UpdateFromKeyboard(false);

            // Place key detection code below this line
            if (input.KeyChar == '1')
                box.getComponents().FirstOrDefault(x => x.getName() == "SWITCH_ONE").UpdateFromKeyboard(true);
            if (input.KeyChar == '2')
                box.getComponents().FirstOrDefault(x => x.getName() == "SWITCH_TWO").UpdateFromKeyboard(true);
            if (input.KeyChar == '3')
                box.getComponents().FirstOrDefault(x => x.getName() == "SWITCH_THREE").UpdateFromKeyboard(true);
            if (input.KeyChar == 'k' || input.KeyChar == 'K')
                box.getComponents().FirstOrDefault(x => x.getName() == "KEY_ANIMATION").UpdateFromKeyboard(true);
            if (input.KeyChar == 'd' || input.KeyChar == 'D')
                box.getComponents().FirstOrDefault(x => x.getName() == "DOOR_ANIMATION").UpdateFromKeyboard(true);
            if (input.KeyChar == 'f' || input.KeyChar == 'F')
                box.getComponents().FirstOrDefault(x => x.getName() == "FUSE_ANIMATION").UpdateFromKeyboard(true);
        }

        private static void OnTimerTick(object sender, ElapsedEventArgs e)
        {
            //Console.WriteLine("OnTimerTick");
            try
            {
                guid = GetGUIDBySceneIDFromVertxAsync();

                Console.WriteLine("GUID : " + guid);
            }
            catch (NullReferenceException exception)
            {
                Console.WriteLine("Null reference exception caught, " + exception.Message);
            }
        }

        static string GetGUIDBySceneIDFromVertxAsync()
        {
            string _guid;
            try
            {
                WebRequest request = WebRequest.Create("https://staging.vertx.cloud/session/scene/" + sceneId);
                // Get the response.  
                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.  
                string responseFromServer = reader.ReadToEnd();
                // Deserialize the repsonse from vertx
                VertxObject responseObj = JsonConvert.DeserializeObject<VertxObject>(responseFromServer);

                Child child = responseObj.rootNode.children.FirstOrDefault(vetxObj => vetxObj.id == "VertxEventManager");
                // Clean up the streams and the response.  
                reader.Close();
                response.Close();

                _guid = child.guid;
                Console.WriteLine("GUID = " + _guid);
            }
            catch (Exception e)
            {
                _guid = null;
                Console.WriteLine(e.Message);
            }

            guid = _guid;

            return _guid;
        }

    }
}
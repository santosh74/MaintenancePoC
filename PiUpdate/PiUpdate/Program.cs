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
        static string sceneId = "083c1f61-97af-4d88-bacf-6664a9690257";
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

            box.Add(new Component("KEY_ANIMATION", "gpio13"));
            box.Add(new Component("SWITCH_ONE", "gpio5"));
            box.Add(new Component("SWITCH_TWO", "gpio4"));
            box.Add(new Component("SWITCH_THREE", "gpio26"));
            box.Add(new Component("DOOR_ANIMATION", "gpio6"));
            box.Add(new Component("FUSE_ANIMATION", "gpio19"));

            WebClient client = new WebClient();
            client.BaseAddress = "https://staging.vertx.cloud";
            client.Headers.Add("Content-Type", "application/json");

            Console.WriteLine("Program ready!\n");

            //Send current box status to unity application
            /*try
            {
                Console.WriteLine(box.getCurrentState());
                Console.WriteLine("Sending current state to VERTX");
                client.UploadData("/session/fire/" + sceneId + "/" + guid + "/OnUpdate", System.Text.UTF8Encoding.UTF8.GetBytes(box.getCurrentState()));
                Console.WriteLine("Data sent to VERTX");
            }
            catch(WebException webException)
            {
                Console.WriteLine("Web exception caught => " + webException.Message);
            }*/

            //Constant service running to check state change
            while (true)
            {
                foreach (Component component in box.getComponents())
                {
                    component.update();
                    bool changed = component.isChanged();

                    if (changed)
                    {
                        //if (guid == null)
                        //{
                        //    try
                        //    {
                        //        guid = GetGUIDBySceneIDFromVertx();
                        //    }
                        //    catch (NullReferenceException exception)
                        //    {
                        //        Console.WriteLine("Null reference exception caught, " + exception.Message);
                        //    }
                        //}//end if
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
                Console.WriteLine(_guid);
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
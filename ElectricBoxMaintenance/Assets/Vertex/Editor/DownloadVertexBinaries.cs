#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System;
using System.Text;

public class DownloadVertexBinaries : MonoBehaviour {

    [MenuItem("VERTX/Get Latest Binaries")]
    public static void GetVertexBinaries()
    {
        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://redmine.vertx.cloud/redmine/projects/vertex-engine/wiki/RELEASE_-_Unity_Plugin");
        request.Method = "GET";

        var response = request.GetResponse() as HttpWebResponse;
        var responseData = new StreamReader(response.GetResponseStream()).ReadToEnd();
        Debug.Log(responseData);
        var pluginPath = Regex.Match(responseData, "/redmine/attachments/download/(.*?)/vertex_unity_plugins.zip");
        if(pluginPath.Success)
        {
            request = (HttpWebRequest)HttpWebRequest.Create("http://redmine.vertx.cloud/" + pluginPath.Value);
            request.ContentType = "application/octet-stream";
            response = request.GetResponse() as HttpWebResponse;
            var zipData = ReadStream(response.GetResponseStream());

            var path = Application.dataPath + "/Vertex/Plugins/";

            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);

            Debug.Log(path + "vertex_unity_plugins.zip");
            File.WriteAllBytes(path + "EXTRACT-THIS-HERE.zip", zipData);
            System.Diagnostics.Process.Start(path);

        }else
        {
            Debug.Log("Could not find binary path");
        }

    }

    public static byte[] ReadStream(Stream input)
    {
        byte[] buffer = new byte[16 * 1024];
        using (MemoryStream ms = new MemoryStream())
        {
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }
            return ms.ToArray();
        }
    }
}
#endif
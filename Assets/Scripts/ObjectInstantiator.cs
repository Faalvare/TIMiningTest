using Newtonsoft.Json;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Dummiesman;
using TiMining.inGame;
namespace TiMining
{
    // Clase que se encarga de importar e instanciar los objetos en la escena basados en la url.
    public class ObjectInstantiator : MonoBehaviour
    {
        private const string objUrl = "https://unity-exercise.dt.timlabtesting.com/data/mesh-obj";
        private const string shovelDataUrl = "https://unity-exercise.dt.timlabtesting.com/data/shovels";
        private const string shovelReportUrl = "https://unity-exercise.dt.timlabtesting.com/data/report";
        public static ShovelReport shovelReports;
        [Tooltip("El shader que se pondrá por defecto al instanciarse")]
        public Shader minePitShader;
        [Tooltip("Prefab de la pala que se instanciará en el rajo")]
        public GameObject shovelPrefab;

        public async void Start()
        {
            /*
             * Consiguiendo json desde los API para luegos deserializarlos
             * se usa await en la llamada a el metodo para la consulta
             * y async en start para hacer todo el proceso asíncrono
             */
            ObjData objData = JsonConvert.DeserializeObject<ObjData>((string) await GetRequest(objUrl));
            ShovelsData shovelDatas = JsonConvert.DeserializeObject<ShovelsData>((string)await GetRequest(shovelDataUrl));
            ShovelReport _shovelReports = JsonConvert.DeserializeObject<ShovelReport>((string)await GetRequest(shovelReportUrl));
            // Descargando archivos  necesarios para instanciar
            string mineObjFilePath = (string)await DownloadFile(objData.ObjUrl, "OpenPitMine.obj");
            string mineMtlFilePath = (string)await DownloadFile(objData.MtlUrl, "OpenPitMine.mtl");
            Texture2D mineTex = (Texture2D)await GetTexture(objData.TextureUrl);
            // Creando los importadores de objetos
            OBJLoader oBJLoader = new OBJLoader();
            MTLLoader mTLLoader = new MTLLoader();
            /* Importando e instanciando el objeto y el material a la vez que asigno el shader al material,la textura al material y el material al objeto,
             * el shader que se usa esta creado en shadergraph y es muy similar al lit shader del universal render pipeline.*/
            var mat = mTLLoader.Load(mineMtlFilePath)["material_0"];
            GameObject mine = oBJLoader.Load(mineObjFilePath);
            mat.shader = minePitShader;
            mat.SetTexture("Base_Map", mineTex);
            mine.GetComponentInChildren<MeshRenderer>().material = mat;
            // Instanciando las palas y asignandoles ID
            foreach (var shovelData in shovelDatas.Shovels)
            {
                var gobj = Instantiate(shovelPrefab, shovelData.Position,Quaternion.identity);
                gobj.GetComponentInChildren<SelectableShovel>().ID = shovelData.ID;
            }
            // Guardando los reportes para uso global
            shovelReports = _shovelReports;



        }

        /// <summary>
        /// Corutina para hacer GET Request devolviendo el string de la respuesta, se usa un asset llamado "Async Await Support" para hacer el proceso asíncrono y a la vez devolver un valor.
        /// </summary>
        /// <param name="uri">url de la consulta</param>
        /// <returns> string conteniendo la respuesta de la consulta </returns>
        IEnumerator GetRequest(string uri)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                // Espera a respuesta de la consulta
                yield return webRequest.SendWebRequest();

                string[] pages = uri.Split('/');
                int page = pages.Length - 1;
                string error = "";
                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                        error = pages[page] + ": Connection Error  " + webRequest.error;
                        yield return error;
                        break;
                    case UnityWebRequest.Result.DataProcessingError:
                        error = pages[page] + ": Error: " + webRequest.error;
                        Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                        yield return error;
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        error = pages[page] + ": HTTP Error: " + webRequest.error;
                        Debug.LogError(error);
                        yield return error;
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                        yield return webRequest.downloadHandler.text;
                        break;
                }
            }
        }

        /// <summary>
        /// Corutina que se usa para descargar archivos.
        /// </summary>
        /// <param name="uri">url del archivo a descargar</param>
        /// <param name="filename">Nombre del archivo</param>
        /// <returns>Ruta del archivo descargado</returns>
        IEnumerator DownloadFile(string uri,string filename)
        {
            var uwr = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbGET);
            string path = Path.Combine(Application.persistentDataPath, filename);
            if (File.Exists(path))
                File.Delete(path);
            uwr.downloadHandler = new DownloadHandlerFile(path);
            yield return uwr.SendWebRequest();
            if (uwr.result != UnityWebRequest.Result.Success)
                Debug.LogError(uwr.error);
            else
            {
                Debug.Log("File successfully downloaded and saved to " + path);
                yield return path;
            }
        }

        /// <summary>
        /// Corutina que se usa para descargar una textura
        /// </summary>
        /// <param name="uri">Url de la textura a descargar</param>
        /// <returns>La textura descargada y lista para usar</returns>
        IEnumerator GetTexture(string uri)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(uri);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                yield return ((DownloadHandlerTexture)www.downloadHandler).texture;
            }
        }

    }

    #region Estructuras de Datos
    // Definiendo estructuras que serán recibidas desde el API
    public struct ObjData
    {
        public string ObjUrl;
        public string MtlUrl;
        public string TextureUrl;
    }

    public struct Shovel
    {
        public int ID;
        public string Name;
        public Vector3 Position;
    }

    public struct ShovelsData
    {
        public Shovel[] Shovels;
    }

    public struct ShovelReportData
    {
        public int ShovelID;
        public float Performance;
        public float PlannedPerformance;
        public States[] LastStates;
    }

    public struct ShovelReport
    {
        public ShovelReportData[] Reports;
    }

    public struct States
    {
        public string Start;
        public string End;
        public string Name;
        public string Color;
    }
    #endregion
}

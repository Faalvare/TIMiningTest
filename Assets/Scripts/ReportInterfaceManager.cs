using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using System;
using TiMining.inGame;

namespace TiMining.UI
{
    // Esta clase se encarga de manegar todo lo relacionado al panel de reportes.
    public class ReportInterfaceManager : MonoBehaviour
    {
        [Tooltip("Referencia al texto que mostrara el ID")]
        public TextMeshProUGUI IDText;
        [Tooltip("Referencia al radial del rendimiento")]
        public Image performanceRadial;
        [Tooltip("Referencia al texto que contiene el radial")]
        public TextMeshProUGUI performanceRadialText;
        [Tooltip("Referencia al contenedor de paneles de estado")]
        public Transform statePanelContainer;
        [Tooltip("Prefab del panel de estados que se instanciarán")]
        public GameObject statePanelPrefab;
        private Coroutine percentFillAnimCR; //Seguimiento de la corutina de la animación del radial, para pararlo en el momento que se requiera

        //Volviendo la clase un Singleton, para acceder sin necesidad de instanciarlo. Ademas no necesito mas instancias de esta interface.
        public static ReportInterfaceManager instance;
        private void Start()
        {
            instance = this;
        }
        private ReportInterfaceManager(){}

        /// <summary>
        /// Despliega la información del reporte basado en el id entregado,
        /// Este es llamado por la pala al ser seleccionada
        /// </summary>
        /// <param name="id">ID de la pala a mostrar reporte</param>
        public void DeployInfo(int id)
        {
            // Consigue el reporte de la pala basado en el ID y setea el texto del ID
            ShovelReportData report = ObjectInstantiator.shovelReports.Reports.Where(report => id == report.ShovelID).FirstOrDefault();
            IDText.text = "ID: "+id;
            /* Setea el porcentaje de rendimiento basado en el rendimiento y rendimiento esperado
             * para luego asignarselos al texto del porcentaje y al radial.*/
            float performance = Mathf.Round(report.Performance * 100 / report.PlannedPerformance);
            performanceRadialText.text = performance + "%";
            percentFillAnimCR = StartCoroutine(PercentFillAnim(performance / 100));
            // Limpiar el panel del historial
            foreach (var item in statePanelContainer.GetComponentsInChildren<StatePanel>())
            {
                Destroy(item.gameObject);
            }
            /*Instanciar paneles de estados y seteando su información.
             * se hace referencia a la clase de statepanel de cada uno de los prefabs para acceder a sus componentes.
             */
            foreach (var state in report.LastStates)
            {
                var panel = Instantiate(statePanelPrefab, statePanelContainer);
                Color circleColor;
                ColorUtility.TryParseHtmlString(state.Color, out circleColor);
                StatePanel panelManager = panel.GetComponent<StatePanel>();
                panelManager.colorCircle.color = circleColor;
                panelManager.stateText.text = state.Name;
                DateTime startTime = Convert.ToDateTime(state.Start);
                DateTime endTime = Convert.ToDateTime(state.End);
                panelManager.ScheduleText.text = "Inicio: " + String.Format("{0:g}", startTime) + "\n"
                   + "Final: " + String.Format("{0:g}", startTime);
            }
        }

        //Ejecuta una animación en el radial, llenando el porcentaje actual.
        private IEnumerator PercentFillAnim(float fillAmount)
        {
            //Parar cualquier animación que ya se este ejecutando
            if (percentFillAnimCR != null)
                StopCoroutine(percentFillAnimCR);
            //Llenando el radial
            float duration = 1.5f;
            float timeElapsed = 0;
            performanceRadial.fillAmount = 0;
            while (fillAmount > performanceRadial.fillAmount)
            {
                Debug.Log("filling " + performanceRadial.fillAmount);
                performanceRadial.fillAmount = Mathf.SmoothStep(0, fillAmount, timeElapsed / duration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            yield return null;
        }

        /// <summary>
        /// Cierra la interface de reportes y deselecciona la pala actualmente seleccionada.
        /// </summary>
        public void Close()
        {
            SelectableShovel.selectedShovel.Deselect();
            gameObject.transform.localScale = Vector3.zero;
        }

        /// <summary>
        /// Abre la interface de reportes
        /// </summary>
        public void Open()
        {
            gameObject.transform.localScale = new Vector3(1,1,1);
        }
    }

}


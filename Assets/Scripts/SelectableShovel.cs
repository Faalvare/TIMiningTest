using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TiMining.UI;

namespace TiMining.inGame
{
    /*
     * Esta clase se encuentra en cada pala y se encarga de aplicar los efectos fresnel a la pala en mouse hover y al seleccionar,
     * ademas de especificar que pala esta seleccionada de forma estatica y enviar los datos que serán mostrados en la interface
     * de reporte.
     */
    public class SelectableShovel : MonoBehaviour
    {
        public Material onHoverMaterial;
        public Material onSelectMaterial;
        public Material defaultMaterial;
        public int ID;
        private bool Selected = false;
        public static SelectableShovel selectedShovel;

        //Aplica efecto fresnel al pasar el mouse por encima.
        void OnMouseEnter()
        {
            if (!Selected)
            {
                foreach (var meshRenderer in GetComponentsInChildren<MeshRenderer>())
                {
                    meshRenderer.material = onHoverMaterial;
                }
            }
        }

        //Vuelve al material por defecto al sacar el mouse si es que no esta seleccionado.
        void OnMouseExit()
        {
            if (!Selected)
            {
                foreach (var meshRenderer in GetComponentsInChildren<MeshRenderer>())
                {
                    meshRenderer.material = defaultMaterial;
                }
            }
        }

        //Selecciona esta pala al clickear el objeto.
        void OnMouseDown()
        {
            Select();
        }

        //Metodo para deseleccionar.
        public void Deselect()
        {
            Selected = false;
            foreach (var meshRenderer in GetComponentsInChildren<MeshRenderer>())
            {
                meshRenderer.material = defaultMaterial;
            }
        }

        /*Metodo para seleccionar, aplicando material con fresnel, deselecciona cualquier otra pala que esté seleccionada
         * y llama el metodo para desplegar la información para la interface, ademas llama el metodo para abrir esta.
         */
        public void Select()
        {
            Selected = true;
            selectedShovel?.Deselect();
            selectedShovel = this;
            foreach (var meshRenderer in GetComponentsInChildren<MeshRenderer>())
            {
                meshRenderer.material = onSelectMaterial;
            }
            ReportInterfaceManager.instance.DeployInfo(ID);
            ReportInterfaceManager.instance.Open();
        }
    }

}

using System.Collections.Generic;
using EjerciciosAlgebra;
using MathDebbuger;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
public class _QuaternionTester : MonoBehaviour
{
    public enum Ejercicio
    {
        Uno,
        Dos,
        Tres
    };
    public Ejercicio ejercicio;
    public float angle;
    void Start()
    {
        VectorDebugger.EnableCoordinates();
        VectorDebugger.EnableEditorView();

        VectorDebugger.AddVector(new Vector3(10f, 0.0f, 0.0f), Color.red, "Uno");
        VectorDebugger.TurnOffVector("Uno");

        List<Vector3> posUno = new List<Vector3>();
        posUno.Add(new Vector3(10f, 0.0f, 0.0f));
        posUno.Add(new Vector3(10f, 10f, 0.0f));
        posUno.Add(new Vector3(20f, 10f, 0.0f));
        VectorDebugger.AddVectorsSecuence(posUno, false, Color.blue, "Dos");
        VectorDebugger.TurnOffVector("Dos");

        List<Vector3> posDos = new List<Vector3>();
        posDos.Add(new Vector3(10f, 0.0f, 0.0f));
        posDos.Add(new Vector3(10f, 10f, 0.0f));
        posDos.Add(new Vector3(20f, 10f, 0.0f));
        posDos.Add(new Vector3(20f, 20f, 0.0f));
        VectorDebugger.AddVectorsSecuence(posDos, false, Color.yellow, "Tres");
        VectorDebugger.TurnOffVector("Tres");
    }
    void Update()
    {
        VectorDebugger.TurnOffVector("Uno");
        VectorDebugger.TurnOffVector("Dos");
        VectorDebugger.TurnOffVector("Tres");
        switch (ejercicio)
        {
            case Ejercicio.Uno:
                VectorDebugger.TurnOnVector("Uno");
                List<Vector3> posListUno = new List<Vector3>();
                for (int i = 0; i < VectorDebugger.GetVectorsPositions("Uno").Count; i++)
                {
                    posListUno.Add(_Quaternion.Euler(0, angle, 0) * VectorDebugger.GetVectorsPositions("Uno")[i]);
                }
                VectorDebugger.UpdatePositionsSecuence("Uno", posListUno);
                break;
            case Ejercicio.Dos:
                VectorDebugger.TurnOnVector("Dos");
                List<Vector3> posListDos = new List<Vector3>();
                for (int i = 0; i < VectorDebugger.GetVectorsPositions("Dos").Count; ++i)
                {
                    posListDos.Add(_Quaternion.Euler(0.0f, angle, 0.0f) * VectorDebugger.GetVectorsPositions("Dos")[i]);
                }
                VectorDebugger.UpdatePositionsSecuence("Dos", posListDos);
                break;
            case Ejercicio.Tres:
                VectorDebugger.TurnOnVector("Tres");
                List<Vector3> posListTres = new List<Vector3>();
                posListTres.Add(VectorDebugger.GetVectorsPositions("Tres")[0]);
                posListTres.Add(_Quaternion.Euler(angle, angle, 0.0f) * VectorDebugger.GetVectorsPositions("Tres")[1]);
                posListTres.Add(VectorDebugger.GetVectorsPositions("Tres")[2]);
                posListTres.Add(_Quaternion.Euler(-angle, -angle, 0.0f) * VectorDebugger.GetVectorsPositions("Tres")[3]);
                posListTres.Add(VectorDebugger.GetVectorsPositions("Tres")[4]);
                VectorDebugger.UpdatePositionsSecuence("Tres", posListTres);
                break;
        }
    }
}
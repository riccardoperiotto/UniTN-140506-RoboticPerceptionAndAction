using UnityEngine;
using Unity.Barracuda;
using TMPro;
using System.Linq;
using System.Collections.Generic;

static class SoftmaxLinqExtension
{
    public static IEnumerable<float> SoftMax(this IEnumerable<float> source)
    {
        var exp = source.Select(x => Mathf.Exp(x)).ToArray();
        var sum = exp.Sum();
        return exp.Select(x => x / sum);
    }
}

public class NumbersScene_Predictor : MonoBehaviour
{
    [SerializeField]
    private NNModel _model;

    [SerializeField]
    private TMP_Text _predictionLabel;
    private Texture2D _texture;
    private IWorker _worker;
    public static NumbersScene_Predictor predictor;


    private void Awake()
    {
        predictor = this;
    }
    void Start()
    {
        _worker = WorkerFactory.CreateWorker(WorkerFactory.Type.Auto, ModelLoader.Load(_model));

        _texture = GetComponent<SpriteRenderer>().sprite.texture;
    }

    public void Predict()
    {
        // Convert the input texture into a 1x28x28x1 tensor.
        using var input = new Tensor(1, 28, 28, 1);

        for (var y = 0; y < 28; y++)
        {
            for (var x = 0; x < 28; x++)
            {
                var tx = x * _texture.width / 28;
                var ty = y * _texture.height / 28;
                input[0, 27 - y, x, 0] = 1 - _texture.GetPixel(tx, ty).grayscale;
                //print("nuovo valore " + _texture.GetPixel(tx, ty).grayscale);
            }
        }


        // start the prediction process
        _worker.Execute(input);


        // Inspect the output tensor.
        var output = _worker.PeekOutput();

        var scores = Enumerable.Range(0, 10).
                     Select(i => output[0, 0, 0, i]).SoftMax().ToList();
        _predictionLabel.text = scores.IndexOf(scores.Max()).ToString();
    }

    public void OnDestroy()
    {
        _worker?.Dispose();
    }


}






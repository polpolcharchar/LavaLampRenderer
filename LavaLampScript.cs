using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BallData
{
    public Vector2 position;
    public float radius;
    public float gravityScale;

    public static int getMemorySize()
    {
        return sizeof(float) * 4;
    }
}

public class LavaLampScript : MonoBehaviour
{

    private RenderTexture renderTexture;
    private Material lavaMaterial;

    public Shader unlitTransparentShader;

    public ComputeShader displayShader;

    public int renderTextureWidth = 1000;
    public int renderTextureHeight = 1000;

    private GameObject[] balls;
    private ComputeBuffer ballBuffer;

    private float[] ballRadii;

    public int numBalls = 10;
    public float ballRadius = 0.04f;
    public GameObject ballPrefab;

    [Range(0.001f, 0.0105f)]
    public float metaballThreshold = 0.0010f;

    public Vector3 coolColor = new Vector3(0, 170, 179);
    public Vector3 heatColor = new Vector3(0, 213, 224);

    public bool chooseRandomColors = false;

    public int colorSmoothingRadius = 10;




    // Start is called before the first frame update
    void Start()
    {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("balls1"), LayerMask.NameToLayer("balls2"));
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("balls2"), LayerMask.NameToLayer("balls3"));
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("balls1"), LayerMask.NameToLayer("balls3"));

        if (chooseRandomColors)
        {
            assignRandomColors();
        }

        displayShader = Instantiate(displayShader);

        displayShader.SetVector("screenDimension", new Vector2(renderTextureWidth, renderTextureHeight));
        displayShader.SetVector("coolColor", new Vector4(coolColor.x, coolColor.y, coolColor.z, 255) / 255f);
        displayShader.SetVector("heatColor", new Vector4(heatColor.x, heatColor.y, heatColor.z, 255) / 255f);

        renderTexture = new RenderTexture(renderTextureWidth, renderTextureHeight, 24);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();

        lavaMaterial = new Material(unlitTransparentShader);
        lavaMaterial.mainTexture = renderTexture;
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = lavaMaterial;

        displayShader.SetTexture(0, "Result", renderTexture);

        createBalls();
    }

    // Update is called once per frame
    void Update()
    {
        displayShader.SetFloat("metaballThreshold", metaballThreshold);
        displayShader.SetInt("colorSmoothingRadius", colorSmoothingRadius);

        sendBallDataToShader();

        displayShader.Dispatch(0, renderTextureWidth / 8, renderTextureHeight / 8, 1);
    }

    void OnDestroy()
    {
        ballBuffer.Dispose();
        renderTexture.Release();
    }

    private void createBalls()
    {
        //instantiate numBalls instances of the ballPrefab
        balls = new GameObject[numBalls];
        ballRadii = new float[numBalls];

        for (int i = 0; i < numBalls; i++)
        {
            // Assuming 'transform' is the parent Transform to which the ball should be relative
            Vector3 localOffset = new Vector3(Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f), 0);
            balls[i] = Instantiate(ballPrefab, transform.TransformPoint(localOffset), Quaternion.identity, transform);

            //balls[i].transform.localScale = new Vector3(Random.Range(ballRadius * 0.5f, ballRadius), Random.Range(ballRadius * 0.5f, ballRadius), 1f);
            balls[i].transform.localScale = new Vector3(ballRadius, ballRadius, 1f);

            ballRadii[i] = Random.Range(ballRadius * 0.5f, ballRadius);

            int layer = Random.Range(1, 4);
            balls[i].layer = LayerMask.NameToLayer("balls" + layer);
        }

        
    }

    private void sendBallDataToShader()
    {
        if(ballBuffer != null)
        {
            ballBuffer.Dispose();
        }


        BallData[] ballData = new BallData[balls.Length];

        for (int i = 0; i < balls.Length; i++)
        {
            Rigidbody2D rb = balls[i].GetComponent<Rigidbody2D>();
            ballData[i].position.x = (balls[i].transform.localPosition.x + 0.5f) * renderTextureWidth;
            ballData[i].position.y = (balls[i].transform.localPosition.y + 0.5f) * renderTextureHeight;
            ballData[i].radius = ballRadii[i] * renderTextureWidth;
            ballData[i].gravityScale = rb.gravityScale / (balls[i].transform.parent.localScale.y / 10) / 0.3f;
        }

        //set the buffer in the shader:
        ballBuffer = new ComputeBuffer(ballData.Length, BallData.getMemorySize());
        ballBuffer.SetData(ballData);
        displayShader.SetBuffer(0, "balls", ballBuffer);

    }

    private void assignRandomColors()
    {
        //set coolColor to a random color, it should not be any shade of grey or almost grey:
        coolColor = new Vector3(Random.Range(0, 255), Random.Range(0, 255), Random.Range(0, 255));
        float multiplier = 1.4f;

        heatColor = new Vector3(coolColor.x * multiplier, coolColor.y * multiplier, coolColor.z * multiplier);
        if (heatColor.x > 255)
        {
            heatColor.x = 255;
        }
        if (heatColor.y > 255)
        {
            heatColor.y = 255;
        }
        if (heatColor.z > 255)
        {
            heatColor.z = 255;
        }
    }
}

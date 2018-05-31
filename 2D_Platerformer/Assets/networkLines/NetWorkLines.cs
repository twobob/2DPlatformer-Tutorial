using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetWorkLines : MonoBehaviour 
{

	public ParticleSystem thisParticleSystem;

	//public TrailRenderer[] theseLines;

	public LineRenderer linePrefab;

	public List<LineRenderer> lines =  new List<LineRenderer>();

	public List<PPL> thisPPL =  new List<PPL>();

	//public int[,,] map;

	private void Start()
	{
		thisParticleSystem = GetComponent<ParticleSystem>();      
	}

	void LateUpdate()
    {
		//loop through all particle systems
        
		int p0;
		int p1;
		int l0;

		ParticleSystem.Particle[] tPS = new ParticleSystem.Particle[thisParticleSystem.particleCount + 1];
		int p = thisParticleSystem.GetParticles(tPS);

		int i = 0;
        while(i < p)
		{
			if (Random.Range(0f,1f) > 0.99f && lines.Count < 30)
			{
				p0 = i;

				p1 = (int)Random.Range(-5, 5) + p0;
				p1 = Mathf.Clamp(p1, 0, p - 1);


				//GameObject GOLR = Instantiate(linePrefab, transform.position, Quaternion.identity) as GameObject;
				//LineRenderer lineRenderer = GOLR.GetComponent<LineRenderer>();

				LineRenderer LR = Instantiate(linePrefab, transform.position, Quaternion.identity) as LineRenderer;

				//LineRenderer LR = gameObject.AddComponent<LineRenderer>();

				LR.gameObject.transform.parent = transform;
				lines.Add(LR);

				l0 = lines.Count - 1;

				PPL newPPL = new PPL();
				newPPL.P0 = p0;
				newPPL.P1 = p1;
				newPPL.L0 = l0;

				thisPPL.Add(newPPL);

				LR.positionCount = 2;
				LR.SetPosition(0, tPS[i].position);
				LR.SetPosition(1, tPS[p1].position);
			}
            
			i++;
		}
        

		if (lines.Count == 0)
		{
			return;
		}

		foreach(PPL q in thisPPL)
		{
			lines[q.L0].SetPosition(0, tPS[q.P0].position);
			lines[q.L0].SetPosition(1, tPS[q.P1].position);
		}
    }
}

[SerializeField]
public class PPL
{
	public int P0;
	public int P1;
	public int L0;
}
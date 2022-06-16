using UnityEngine;
using UnityEngine.Pool;

public class ParticleSpawner : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;
    private ObjectPool<ParticleSystem> particlesPool;

    private void Awake()
    {
        particlesPool = new ObjectPool<ParticleSystem>(
            createFunc: CreateParticle,
            actionOnGet: OnTakeParticleFromPool,
            actionOnRelease: OnReturnParticleToPool);

        //Prewarm pool
        PreWarmPool(4);
    }

    private void OnEnable()
    {
        GameManager.PlayerFoundTilePair += OnPlayerFoundTilePair;
    }
    private void OnDisable()
    {
        GameManager.PlayerFoundTilePair -= OnPlayerFoundTilePair;
    }

    private void OnPlayerFoundTilePair(object sender, (Tile, Tile) tilePair)
    {
        ParticleSystem particle1 = particlesPool.Get();
        ParticleSystem particle2 = particlesPool.Get();

        particle1.transform.position = tilePair.Item1.transform.position;
        particle2.transform.position = tilePair.Item2.transform.position;
    }

    private ParticleSystem CreateParticle()
    {
        ParticleSystem newParticle = Instantiate(particles);
        newParticle.transform.parent = this.transform;

        // This is used to return ParticleSystems to the pool when they have stopped.
        ReturnToPool returnToPool = newParticle.gameObject.AddComponent<ReturnToPool>();
        returnToPool.pool = particlesPool;

        return newParticle;
    }

    private void OnTakeParticleFromPool(ParticleSystem particle)
    {
        particle.gameObject.SetActive(true);
        //particle.Play();
    }
    private void OnReturnParticleToPool(ParticleSystem particle)
    {
        particle.gameObject.SetActive(false);
    }

    public ParticleSystem GetParticle() => particlesPool.Get();

    private void PreWarmPool(int prewarmAmount)
    {
        ParticleSystem[] preWarmArray = new ParticleSystem[prewarmAmount];
        for (int i = 0; i < prewarmAmount; i++)
        {
            ParticleSystem newParticleSystem = particlesPool.Get();
            preWarmArray[i] = newParticleSystem;
        }
        for (int i = 0; i < preWarmArray.Length; i++)
        {
            particlesPool.Release(preWarmArray[i]);
        }
    }

}

// This component returns the particle system to the pool when the OnParticleSystemStopped event is received.
[RequireComponent(typeof(ParticleSystem))]
public class ReturnToPool : MonoBehaviour
{
    public ParticleSystem system;
    public IObjectPool<ParticleSystem> pool;

    void Start()
    {
        system = GetComponent<ParticleSystem>();
        var main = system.main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    void OnParticleSystemStopped()
    {
        // Return to the pool
        pool.Release(system);
    }
}

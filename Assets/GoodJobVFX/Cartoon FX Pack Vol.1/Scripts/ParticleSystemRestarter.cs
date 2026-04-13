using UnityEngine;

namespace CartoonFXPackVol1
{
    public class ParticleSystemRestarter : MonoBehaviour
    {
        private ParticleSystem[] allParticleSystems;

        void Start()
        {
            allParticleSystems = FindObjectsOfType<ParticleSystem>();
            StartCoroutine(RestartParticleSystems());
        }

        private System.Collections.IEnumerator RestartParticleSystems()
        {
            while (true)
            {
                yield return new WaitForSeconds(8f);
                foreach (ParticleSystem ps in allParticleSystems)
                {
                    ps.Clear();
                    ps.Play();
                }
            }
        }
    }
}
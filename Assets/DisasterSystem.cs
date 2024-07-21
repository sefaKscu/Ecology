using System.Linq;
using UnityEngine;

namespace Ecology
{
    public class DisasterSystem : MonoBehaviour
    {
        [SerializeField] UnitProfile[] ProfilesToHit = new UnitProfile[0];
        [SerializeField, Range(0, 1)] public float disaterStructChance = 0.05f;
        [SerializeField] ParticleSystem effect;

        private void Awake()
        {
            if (effect == null) effect = GetComponentInChildren<ParticleSystem>();
        }

        public void IncrementTick()
        {
            if (disaterStructChance >= Random.Range(0f, 1f))
            {
                StartDisaster();
            }
        }
        BoxCollider2D[] hitResultOfDisaster = new BoxCollider2D[0];
        void StartDisaster()
        {
            Debug.Log("<color=red>A DISASTER STRUCK!!</color>");
            Vector2 position = new Vector2(Random.Range(-50, 50),Random.Range(-30, 30));
            float radius = Random.Range(5, 15);
            Debug.Log(position);

            effect.transform.position = position;
            effect.Emit(50);
            Collider2D[] hits = Physics2D.OverlapCircleAll(position, radius);
            foreach (Collider2D hit in hits)
            {
                if (ProfilesToHit.Contains(hit.GetComponent<Unit>().Profile))
                {
                    Destroy(hit.gameObject);
                }
            }
        }
    }
}

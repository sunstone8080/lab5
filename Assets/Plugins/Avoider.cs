using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AvoiderPlugin
{
    public class Avoider : MonoBehaviour
    {
        public Transform avoidee;
        public float range = 10f;
        public float speed = 5f;
        public bool showGizmos = true;

        NavMeshAgent agent;
        readonly List<Vector3> visiblePoints = new List<Vector3>();
        readonly List<Vector3> hiddenPoints = new List<Vector3>();




        void OnValidate()
        {
            if (GetComponent<NavMeshAgent>() == null)
            {
                Debug.LogWarning("Avoider needs a NavMeshAgent", this);


            }

            if (avoidee == null)
            {
                Debug.LogWarning("Avoider needs an avoidee", this);


            }
        }

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();

            if (agent == null)
            {
                Debug.LogWarning("Avoider:no NavMeshAgent found", this);

                enabled = false;
                return;
            }

            agent.updateRotation = false;
            StartCoroutine(AvoidLoop());


        }

        void Update()
        {
            if (avoidee == null)
            {
                return;


            }

            Vector3 dir = avoidee.position - transform.position;
            dir.y = 0f;

            if (dir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(dir);


            }


        }

        IEnumerator AvoidLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.25f);

                if (avoidee == null)
                {
                    continue;


                }

                if (Vector3.Distance(transform.position, avoidee.position) > range)
                {
                    continue;


                }

                if (!IsVisible(transform.position))
                {
                    continue;


                }

                FindPoints();

                if (hiddenPoints.Count == 0)
                {
                    continue;


                }

                agent.speed = speed;
                agent.SetDestination(Closest());


            }
        }

        void FindPoints()
        {
            visiblePoints.Clear();
            hiddenPoints.Clear();

            var sampler = new PoissonDiscSampler(range * 2f, range * 2f, range / 5f);

            foreach (var s in sampler.Samples())
            {


                Vector3 p = transform.position + new Vector3(s.x - range, 0f, s.y - range);

                NavMeshHit hit;
                if (!NavMesh.SamplePosition(p, out hit, 3f, NavMesh.AllAreas))
                {
                    continue;

                }

                if (IsVisible(hit.position))
                {
                    visiblePoints.Add(hit.position);

                }
                else
                {
                    hiddenPoints.Add(hit.position);


                }


            }
        }

        bool IsVisible(Vector3 point)
        {
            RaycastHit hit;

            if (Physics.Linecast(avoidee.position, point + Vector3.up * 0.5f, out hit))
            {
                return hit.transform == transform;


            }

            return true;
        }

        Vector3 Closest()
        {
            Vector3 best = hiddenPoints[0];

            foreach (var p in hiddenPoints)
            {
                if ((p - transform.position).sqrMagnitude < (best - transform.position).sqrMagnitude)
                {
                    best = p;


                }


            }

            return best;
        }

        void OnDrawGizmos()
        {
            if (!showGizmos)
            {
                return;


            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, range);

            Gizmos.color = Color.red;
            foreach (var p in visiblePoints)
            {
                Gizmos.DrawLine(transform.position, p);


            }

            Gizmos.color = Color.green;
            foreach (var p in hiddenPoints)
            {
                Gizmos.DrawLine(transform.position, p);


            }
        }
    }
}
using UnityEngine;
using System.Collections.Generic;
using Scripts.Event_Dispatchers;

namespace Scripts.Enemy_Characters
{
    public class EnemyCharacterTargetSelector : MonoBehaviour
    {
        public delegate void TargetAssignmentEvent(Transform requester, Transform target);
        public static event TargetAssignmentEvent TargetAssignmentHandler;

        private List<Transform> _allies;
        private List<Transform> _enemies;

        public EnemyCharacterTargetSelector() : base()
        {
            _allies = new List<Transform>();
            _enemies = new List<Transform>();
        }

        private void OnEnable()
        {
            StatusEventDispatcher.StatusEventHandler += HandleStatusEvent;
        }

        private void OnDisable()
        {
            StatusEventDispatcher.StatusEventHandler -= HandleStatusEvent;
        }

        private void Awake()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);

                if (child.tag.StartsWith("Player"))
                {
                    _enemies.Add(child);
                }
                else if (child.tag.StartsWith("Enemy"))
                {
                    _allies.Add(child);
                }
            }

            Debug.Log(string.Format("Found {0} PCs and {1} NPCs", _enemies.Count, _allies.Count));
        }

        private void HandleStatusEvent(Transform originator, Transform target, StatusMessage message, float value)
        {
            switch (message)
            {
                case StatusMessage.NpcActionTargetRequested: AttemptTargetAssignment(originator); break;
                case StatusMessage.CharacterDead: RemovePotentialTarget(originator); break;
            }
        }

        private void AttemptTargetAssignment(Transform requestingNpc)
        {
            List<Transform> targetList = CharacterUtilities.CharacterTargetsAllies(requestingNpc) ? _allies : _enemies;
            Transform actionTarget = SelectTarget(targetList, requestingNpc);

            if ((actionTarget != null) && (TargetAssignmentHandler != null))
            {
                Debug.Log("Assigning target " + actionTarget.name + " to " + requestingNpc.name);
                TargetAssignmentHandler(requestingNpc, actionTarget);
            }
        }

        private Transform SelectTarget(List<Transform> targetList, Transform requestingNpc)
        {
            Transform target = null;
            int index = Random.Range(0, targetList.Count);
            int steps = 0;

            while ((target == null) && (steps < targetList.Count))
            {
                if ((targetList[index] != requestingNpc) && (targetList[index].gameObject.activeInHierarchy))
                {
                    target = targetList[index];
                }
                else
                {
                    index = (index + 1) % targetList.Count;
                    steps++;
                }
            }

            return target;
        }

        private void RemovePotentialTarget(Transform deadCharacter)
        {
            if (_allies.Contains(deadCharacter)) 
            {
                Debug.Log(deadCharacter.name + " removed from NPC allies list");
                _allies.Remove(deadCharacter); 
            }

            if (_enemies.Contains(deadCharacter))
            {
                Debug.Log(deadCharacter.name + " removed from NPC enemies list");
                _enemies.Remove(deadCharacter);
            }
        }
    }
}

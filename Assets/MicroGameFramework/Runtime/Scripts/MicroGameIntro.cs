using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class MicroGameIntro : MonoBehaviour
    {
        public float _duration;

        public GameObject _titleCard;
        public Text _text;

        public void ApplyTitle(MicroGameHandler game)
        {
            _text.text = game._description;
        }


        public IEnumerator DisplayTitle(MicroGameHandler game)
        {
            _text.text = game._description;
            _titleCard.SetActive(true);
            yield return new WaitForSeconds(_duration);
            _titleCard.SetActive(false);
        }

    }
}
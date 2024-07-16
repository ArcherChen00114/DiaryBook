namespace echo17.EndlessBook.Demo02
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using echo17.EndlessBook;
    using UnityEngine.UI;


    /// <summary>
    /// View of an animating book on the front page
    /// </summary>
    public class CustomPage : PageView
    {
        
        protected override bool HandleHit(RaycastHit hit, BookActionDelegate action)
        {
            
            Debug.Log(hit.collider.gameObject.name);
            if (hit.collider.gameObject.GetComponent<Button>() != null)
            {
                hit.collider.gameObject.GetComponent<Button>().onClick.Invoke();
            }
            return true;
        }

    }
}

using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Animator anim;
    public char orientation;

    // Start is called before the first frame update
    void Start() {
        anim = GetComponent<Animator>();
        orientation = 'F';
    }

    // Update is called once per frame
    void Update() {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 mov = new Vector3(h, v, 0);

        if (mov.magnitude > 0) {
            transform.Translate(mov * 60f * Time.deltaTime, Space.World);
        }

        if (Input.GetKey(KeyCode.W)) {
            anim.SetBool("isMoving", true);
            anim.SetInteger("horizontal", 0);
            anim.SetInteger("vertical", 1);

            orientation = 'B';
            return;
        }

        if (Input.GetKey(KeyCode.A)) {
            anim.SetBool("isMoving", true);
            anim.SetInteger("horizontal", -1);
            anim.SetInteger("vertical", 0);

            orientation = 'L';
            return;
        }

        if (Input.GetKey(KeyCode.S)) {
            anim.SetBool("isMoving", true);
            anim.SetInteger("horizontal", 0);
            anim.SetInteger("vertical", -1);

            orientation = 'F';
            return;
        }

        if (Input.GetKey(KeyCode.D)) {
            anim.SetBool("isMoving", true);
            anim.SetInteger("horizontal", 1);
            anim.SetInteger("vertical", 0);

            orientation = 'R';
            return;
        }

        anim.SetBool("isMoving", false);
        anim.SetInteger("horizontal", 0);
        anim.SetInteger("vertical", 0);
    }
}

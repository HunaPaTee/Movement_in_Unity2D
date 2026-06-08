using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Thong So He Thong")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    public float climbSpeed = 5f; // Tốc độ leo thang
    
    [Header("Kiem Tra Mat Dat")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    
    // Biến trạng thái
    private float horizontalInput;
    private float verticalInput;
    private bool isGrounded;
    private bool isNearLadder;
    private bool isClimbing;
    private float defaultGravity;
    
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravity = rb.gravityScale; // Lưu lại trọng lực gốc
    }

    void Update()
    {
        // Nhận tín hiệu điều hướng
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Cảm biến mặt đất
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.5f, groundLayer);

        // Logic Nhảy
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded && !isClimbing)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // --- ĐÂY CHÍNH LÀ MODULE LEO THANG BỊ MẤT TÍCH ---
        if (isNearLadder && Mathf.Abs(verticalInput) > 0.1f)
        {
            isClimbing = true; // Kích hoạt trạng thái leo trèo
        }
    }
    void FixedUpdate()
    {
        // Chuyển đổi trạng thái Vật lý
        if (isClimbing)
        {
            rb.gravityScale = 0f; // Tắt trọng lực
            // Gán cứng vận tốc theo input để nhân vật dừng lại trên thang khi nhả phím
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, verticalInput * climbSpeed);
        }
        else
        {
            rb.gravityScale = defaultGravity; // Trả lại trọng lực
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        }
    }

    // Module Cảm biến: Khi bước vào vùng Thang
    // Cực kỳ lưu ý: 2 hàm này phải nằm ĐỘC LẬP bên ngoài Update và FixedUpdate nhé!
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Khi nhân vật chạm vào bất cứ thứ gì là Trigger, nó sẽ in ra dòng này:
        Debug.Log("Hệ thống chạm vào vật thể có Tag là: " + collision.gameObject.tag);
        
        if (collision.CompareTag("Ladder"))
        {
            Debug.Log("CẢM BIẾN BÁO: Nhân vật đã vào vùng Cầu Thang!");
            isNearLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            Debug.Log("CẢM BIẾN BÁO: Nhân vật đã thoát khỏi Cầu Thang!");
            isNearLadder = false;
            isClimbing = false;
        }
    }
}
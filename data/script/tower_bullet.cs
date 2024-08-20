using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

// ����� ���������� ������

[Component(PropertyGuid = "ee7abc98c9ee74fbb4152b64ffb3e25dc0b968d1")]
public class tower_bullet : Component
{
    // ������ ��� �� �������� �����

    public float visibility_distance = 30; // ��������� ��������� �����
    public float reload_time = 0.1f; // ����� �����������

    public Node rotate_node;  // ��� ���� �������
    public Node muzzle; // ���� ����� ��������

    public float rotation_speed = 90; // �������� �������� deg/sec 

    public AssetLink bullet_type; // ������ �� ���� � ��������

    // ���� �� ������� �� ����������. ��� ������� ���� � ����� �����, ��� ��������� ���� ������� ����� ������, ����� �������
    public bool aim_advance = true; 

    private mosquitto target;
    private WorldBoundSphere bs;
    private float timer = 0.0f;

    private dvec3 target_pos_old = dvec3.ZERO;
    private float bullet_speed = 1.0f;

    void Init()
    {
        bs = new WorldBoundSphere(node.WorldPosition, visibility_distance);

        // ��������� ���� ������, ����� ����� ������� �������� ���� �������� (����� ��� �������� �� ����������) � ������� ��� ����� �� �������
        Node n = World.LoadNode(bullet_type.Path);
        bullet b = GetComponent<bullet>(n);
        bullet_speed = b.speed;
        n.DeleteLater();
    }

    void Shoot() // ��������
    {
        Node n = World.LoadNode(bullet_type.Path); // ��������� ������
        n.WorldTransform = muzzle.WorldTransform; // ������ ������ �� ����� �����
    }

    // � �� ���� ��� ��� �� �������, � ������� � chatGPT3.5 �� �������. ����� ��������
    public static dvec3 CalculateLeadingPosition(dvec3 shooterPosition, dvec3 targetPosition, vec3 targetVelocity, float bulletSpeed)
    {
        // Relative position and velocity
        vec3 relativePosition = (vec3)(targetPosition - shooterPosition);

        float a = MathLib.Dot(targetVelocity, targetVelocity) - bulletSpeed * bulletSpeed;
        float b = 2.0f * MathLib.Dot(targetVelocity, relativePosition);
        float c = MathLib.Dot(relativePosition, relativePosition);

        // Solve quadratic equation a*t^2 + b*t + c = 0
        float discriminant = b * b - 4 * a * c;

        if (discriminant < 0)
        {
            // No real solution, can't hit the target
            return targetPosition; // Just return the current position of the target
        }

        // Calculate the two possible solutions
        float sqrtDiscriminant = (float)Math.Sqrt(discriminant);
        float t1 = (-b - sqrtDiscriminant) / (2 * a);
        float t2 = (-b + sqrtDiscriminant) / (2 * a);

        // We need the smallest positive time
        float t = Math.Max(t1, t2);
        if (t < 0)
        {
            t = Math.Max(t1, t2);
        }

        // Calculate the leading position
        dvec3 leadingPosition = targetPosition + targetVelocity * t;
        return leadingPosition;
    }

    void Update()
    {
        if (target && !target.node)
        {
            target = null;
        }

        // �� �������� � �������� ������ - ���� ���������� ������
        if (!target)
        {
            double distance = visibility_distance * visibility_distance;
            List<Node> nodes = new List<Node>();
            World.GetIntersection(bs, nodes);
            foreach (Node n in nodes)
            {
                mosquitto m = GetComponentInParent<mosquitto>(n);
                if (m)
                {
                    double l2 = MathLib.Distance2(n.WorldPosition, node.WorldPosition);
                    if (l2 < distance)
                    {
                        distance = l2;
                        target = m;
                    }
                }
            }
        }

        if (target)
        {
            // ����� ������� ���� � �� ��������
            dvec3 target_pos = target.node.WorldPosition;
            vec3 speed = (vec3)(target_pos - target_pos_old) / Game.IFps;
            target_pos_old = target_pos;

            // ���� ���� �������� � ��������������� - ������� ������� ���� ��� ��� ����������� � �����
            if (aim_advance)
                target_pos = CalculateLeadingPosition(rotate_node.WorldPosition, target_pos, speed, bullet_speed);

            // ����� ����������� �� ����
            vec3 dir_to_target = (vec3)(target_pos - rotate_node.WorldPosition);
            dir_to_target.Normalize();

            // ������������ �� ���� ��� �� ��� lazer tower
            var target_rotation = MathLib.RotationFromDir(dir_to_target, vec3.UP);
            var new_rotation = MathLib.RotateTowards(rotate_node.GetWorldRotation(), target_rotation, rotation_speed * Game.IFps);
            rotate_node.SetWorldRotation(new_rotation, true);

            // ���� ����� �������� � �������� �� ���� - ���!
            if (timer <= 0 && MathLib.Equals(dir_to_target, rotate_node.GetWorldDirection(MathLib.AXIS.Y), 0.01f))
            {
                Shoot();
                timer = reload_time;
            }
        }

        if (timer > 0)
            timer -= Game.IFps;
    }
}
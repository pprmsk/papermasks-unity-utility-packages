<div align="center">

# PAPERMASK's USEFUL ATTRIBUTES

<img width="200" alt="pprmsk-ezgif com-resize" src="https://github.com/user-attachments/assets/031b58d3-b3c0-4315-821d-6a359f68a592" />

A lightweight collection of custom **editor attributes and drawers** to improve your Unity Inspector experience. <br>
Each script adds either visual polish, validation, or convenient functionality to serialized fields, without any heavy dependencies.

GIT LINK: **`https://github.com/pprmsk/papermasks-unity-utilities.git?path=UsefulAttributes`**

</div>

---

## Feature Overview

### **Attributes**

| Attribute | Description |
|------------|--------------|
| **ButtonAttribute** | Adds a clickable button to the Inspector that calls a method defined using lambda. |
| **HideIfAttribute** | Conditionally hides a field in the Inspector based on another field or property value. |
| **NullValueLabelAttribute** | Displays a custom label when a serialized reference is `null`. |
| **PrefixAttribute** | Allows you to prefix fields with custom labels or icons for readability. |
| **ReadOnlyAttribute** | Makes serialized fields visible but non-editable in the Inspector. |
| **RichHeaderAttribute** | Adds rich-text supported headers above field groups. |
| **ValidateAttribute** | Adds validation based on validation types and shows custom Debug errors when field doesn't meet validation criteria. |

---

## **Editor Drawers**

Each attribute comes with a corresponding custom **PropertyDrawer** or **Editor** class:

| Drawer / Editor | Related Attribute |
|------------------|--------------------|
| `ButtonAttributeEditor.cs` | `ButtonAttribute` |
| `HideIfDrawer.cs` | `HideIfAttribute` |
| `NullValueLabelDrawer.cs` | `NullValueLabelDrawer` |
| `PrefixDrawer.cs` | `PrefixDrawer` |
| `ReadOnlyDrawer.cs` | `ReadOnlyAttribute` |
| `RichHeaderDrawer.cs` | `RichHeaderDrawer` |
| `ValidateDrawer.cs` | `ValidateAttribute` |

---

## Usage Examples

### Button Attribute
```cs
[Button(group:"1")] void LockPlayer() => LockMovement();

void LockMovement() { canMove = false; }
```

### HideIf Attribute
```cs
public bool HasShooting { get; private set; }

[HideIf(nameof(HasShooting), false)] public Transform projectileSpawnPoint;
```

### Validate Attribute
```cs
[Validate(ValidateType.NotNull)] public GameObject prefab;
```

### Rich Header Attribute
```cs
[RichHeader("<color=red>[OPTIONAL]</color>")]
```

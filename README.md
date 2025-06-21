# 🧠 Note Client UI (C# Desktop App)

![{7204DB0C-B5BC-4869-B835-A104742B480A}](https://github.com/user-attachments/assets/94954d1e-b91d-4df7-84c0-f01cd7c71312)

This is a lightweight encrypted note-taking desktop application built in C#. It connects to a remote server using a custom API and synchronizes encrypted note files. This document describes the UI layout, behavior, encryption model, and known platform-specific issues.

---

![{06D9C296-1DCC-45A7-BE90-B4085C28863D}](https://github.com/user-attachments/assets/aa1dc451-ad02-4d72-a97f-54a3144eaba3)

## 📦 JSON Note Object Structure

Notes are stored and transferred as JSON objects, for example:

```json
{
  "Title": "exampleNote",
  "InternalData": "+NUx6oM=",
  "InitVector": "tbUJiSK9HDWv7nX3",
  "SecurityTag": "kfWTqDHCjixt8s+P6BS62g==",
  "LastModified": "2024-09-13T15:31:44.6367454+01:00",
  "Upload": true
}
```

---

## 🖥️ User Interface Overview

### 🧾 Sidebar - Note List

* Lists all notes by `Title`
* Loads note content into the editor when selected
* Displays sync status icons:

  * ✅ Synced with server
  * ⚠️ Unsynced (if `Upload` is `false`)

### ✍️ Editor Pane

* **Title** field: editable
* **Body** field: multiline plaintext editor
* **LastModified**: shown as read-only timestamp
* **Upload toggle**: determines whether the note syncs to the server

---

## 🔄 Synchronization Workflow

1. Note modified in the UI
2. `SetText()` is triggered

   * Encrypts content using AES-GCM with a unique InitVector and SecurityTag
   * Updates `InternalData`, `LastModified`, etc.
3. If `Upload = true` → note is sent to the server via `PostNote`
4. If `Upload = false` → note is saved locally to `/Notes`

---

## 🧪 Known Bug: C# Version UI Rendering Issue

A recent .NET runtime update caused a rendering bug where:

* Certain line endings may be cut off or invisible
* Affects `TextBox` and `RichTextBox` controls
* Does **not** affect saved files or server communication

### Workaround

* Programmatically appending whitespace to bugged lines
* Considering a migration to custom rendering or alternative frameworks

---

![{5D674AC0-FE72-4A33-B632-642FE992FA73}](https://github.com/user-attachments/assets/5071e547-0e83-4968-8027-55091a48fa2b)

## 🔐 Encryption Details

* **Cipher:** AES-GCM (256-bit)
* **Key derivation:** `PasswordHashing.DeriveHash`
* **Key sources:**

  * Password (plaintext input)
  * Email (used as salt)
* **Encryption process:**

  * Each note is encrypted with a unique InitVector and SecurityTag
  * `InternalData` stores encrypted content

---

![{535F298E-2146-4DE2-A4FB-B6E4663BAC8C}](https://github.com/user-attachments/assets/95208695-2431-4184-8e55-bbf58b04125d)

![{1F827A7A-D66C-47C3-BFAB-B1126FCB06CB}](https://github.com/user-attachments/assets/9834117f-c94d-4a8f-97e9-cdd3038d52b8)

## 🗂 Local File Management

* Notes are saved as `<Title>.json` in `/Notes` directory
* `GetNoteTitles` API is used to populate the sidebar
* If offline, notes can be saved locally and synced later
* Deletion in UI removes the JSON file from the disk

---

## ⚙️ Additional Features

* 🔍 Search bar for note titles
* 📊 Sort options (alphabetical, last modified, sync status)
* ⚙️ Settings menu:

  * Login/Logout
  * Change encryption key
  * Toggle automatic syncing
* 🟢 Status bar:

  * Session info (e.g., email)
  * Sync and connection status

---

![{A9DCFA2F-619E-4E42-892D-56D4A21BA378}](https://github.com/user-attachments/assets/c401e2e7-6f4a-44f3-b472-f6b6ca2e1492)

## 🧩 Tech Stack

* Language: C# (.NET)
* UI: WinForms or WPF (based on implementation)
* Storage: Local JSON file storage
* Networking: `HttpClient` with POST/GET API calls

---

Feel free to open an issue if you run into UI rendering bugs or want to contribute to the project.

import React, { useState } from 'react'
import axios from 'axios'

const API_BASE = import.meta.env.VITE_API_BASE || 'http://localhost:5000/api'

export default function NewIncidentForm({ onCreated }) {
  const [title, setTitle] = useState('')
  const [description, setDescription] = useState('')
  const [severity, setSeverity] = useState('Medium')
  const [files, setFiles] = useState(null)

  const submit = async (e) => {
    e.preventDefault()
    const form = new FormData()
    form.append('title', title)
    form.append('description', description)
    form.append('severity', severity)
    if (files) {
      for (const f of files) form.append('files', f)
    }
    await axios.post(`${API_BASE}/incidents`, form, { headers: { 'Content-Type': 'multipart/form-data' } })
    setTitle('')
    setDescription('')
    setFiles(null)
    onCreated()
  }

  return (
    <form onSubmit={submit} className="new-incident">
      <input value={title} onChange={(e) => setTitle(e.target.value)} placeholder="Title" required />
      <textarea value={description} onChange={(e) => setDescription(e.target.value)} placeholder="Description" />
      <select value={severity} onChange={(e) => setSeverity(e.target.value)}>
        <option>Low</option>
        <option>Medium</option>
        <option>High</option>
      </select>
      <input type="file" multiple onChange={(e) => setFiles(e.target.files)} />
      <button type="submit">Create Incident</button>
    </form>
  )
}

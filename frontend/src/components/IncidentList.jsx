import React from 'react'
import axios from 'axios'

const API_BASE = import.meta.env.VITE_API_BASE || 'http://localhost:5000/api'

export default function IncidentList({ incidents, onRefresh }) {
  const updateStatus = async (id, status) => {
    await axios.patch(`${API_BASE}/incidents/${id}/status`, JSON.stringify(status), { headers: { 'Content-Type': 'application/json' } })
    onRefresh()
  }

  return (
    <div className="incident-list">
      {incidents && incidents.map(i => (
        <div key={i.id} className="incident-card">
          <h3>{i.title}</h3>
          <p>{i.description}</p>
          <div>Severity: {i.severity}</div>
          <div>Status: {i.status}</div>
          <div>CreatedAt: {(new Date(i.createdAt)).toLocaleString()}</div>
          <div className="actions">
            <button onClick={() => updateStatus(i.id, 'InProgress')}>In Progress</button>
            <button onClick={() => updateStatus(i.id, 'Resolved')}>Resolve</button>
          </div>
          {i.attachments && i.attachments.map(a => (
            <div key={a.id}><a href={a.blobUrl} target="_blank" rel="noreferrer">{a.fileName}</a></div>
          ))}
        </div>
      ))}
    </div>
  )
}

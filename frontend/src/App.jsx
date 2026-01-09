import React, { useEffect, useState } from 'react'
import axios from 'axios'
import IncidentList from './components/IncidentList'
import NewIncidentForm from './components/NewIncidentForm'

const API_BASE = import.meta.env.VITE_API_BASE || 'http://localhost:5000/api'

export default function App() {
  const [incidents, setIncidents] = useState([])

  const fetchIncidents = async () => {
    const res = await axios.get(`${API_BASE}/incidents`)
    setIncidents(res.data.items)
  }

  useEffect(() => { fetchIncidents() }, [])

  return (
    <div className="container">
      <h1>Incident Management</h1>
      <NewIncidentForm onCreated={fetchIncidents} />
      <IncidentList incidents={incidents} onRefresh={fetchIncidents} />
    </div>
  )
}

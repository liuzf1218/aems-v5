import axios from 'axios'

const request = axios.create({
  baseURL: 'https://localhost:5001',
  timeout: 5000,
})

request.interceptors.response.use((res) => res.data)
export default request

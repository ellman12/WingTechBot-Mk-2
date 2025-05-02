import axios from "axios";

const apiUrl = import.meta.env.VITE_API_URL;

const http = axios.create({
    baseURL: `${apiUrl}/api`,
    headers: {"Content-Type": "application/json"}
});

export default http;

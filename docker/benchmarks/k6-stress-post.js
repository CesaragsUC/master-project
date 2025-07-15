import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    vus: 200,
    duration: '10m',
};

export default function () {
	
    // Exemplo de payload para o POST (ajuste conforme sua API)
    const payload = JSON.stringify({
        name: "Xbox Series X",
        price: 4800,
		active: true,
		imageBase64: ""
    });

    const params = {
        headers: {
            'Authorization': 'eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICIwYTRtc05IMzF4QldvcUZWaWl4UmRzY2tUd1BPbFJyVHl2SWFjSVBnSDlVIn0.eyJleHAiOjE3NTE2MjkwMjUsImlhdCI6MTc1MTU5MzAyNSwianRpIjoiZDAxNGI0MmYtYWQyZi00NjBlLWIwYWEtMTQ0NzM4NjI5MjA5IiwiaXNzIjoiaHR0cDovL2Nhc29mdC1zdG9yZS1rZXljbG9hazo4MDgwL3JlYWxtcy9jYXNvZnQiLCJhdWQiOlsiY2Fzb2Z0c3lzdGVtIiwiYWNjb3VudCJdLCJzdWIiOiI0MjVhNGYyZS05ODJlLTQxNmEtOWY2YS00MGYzMTBjN2M3YmUiLCJ0eXAiOiJCZWFyZXIiLCJhenAiOiJjYXNvZnRzeXN0ZW0iLCJzaWQiOiIyMTg2ZDBlNC1jNjQ2LTRjMGYtYjBmYS1lYWUzNzZkZWY4ZjUiLCJhY3IiOiIxIiwiYWxsb3dlZC1vcmlnaW5zIjpbIioiXSwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbIm9mZmxpbmVfYWNjZXNzIiwiZGVmYXVsdC1yb2xlcy1jYXNvZnQiLCJ1bWFfYXV0aG9yaXphdGlvbiJdfSwicmVzb3VyY2VfYWNjZXNzIjp7ImNhc29mdHN5c3RlbSI6eyJyb2xlcyI6WyJSZWFkIiwiRGVsZXRlIiwiQ3JlYXRlIiwiVXBkYXRlIiwiQWRtaW4iXX0sImFjY291bnQiOnsicm9sZXMiOlsibWFuYWdlLWFjY291bnQiLCJtYW5hZ2UtYWNjb3VudC1saW5rcyIsInZpZXctcHJvZmlsZSJdfX0sInNjb3BlIjoib3BlbmlkIHByb2ZpbGUgZW1haWwiLCJlbWFpbF92ZXJpZmllZCI6ZmFsc2UsIm5hbWUiOiJDZXNhciBBdWd1c3RvIiwicHJlZmVycmVkX3VzZXJuYW1lIjoiY2VzYXIiLCJnaXZlbl9uYW1lIjoiQ2VzYXIiLCJmYW1pbHlfbmFtZSI6IkF1Z3VzdG8iLCJlbWFpbCI6ImNlc2FyQGRlbW8uY29tIn0.dDvyRlMacAzNZtvG8JylSq0nWO7c6SuZ7Omf5mlxjV3pMBIpvl0R79tYO2kXMLWlA_PLPZ631xWkyA3PTZfl52QuL0DMCvRWWC3aM7P-Dv7F-hNWtLgRJpAIh9VlYjfNVVLR_n3ANV3vzBm-Zeaz0JJjKDpQmZ-qyT5_Ip76aQ_V2p2_OoC_CGd1D7LPMPUbC6hUsE38EUdFfl4nXoglDG8_CD6WUmL1SNlml-6a9eDnCGa9MFSh5WCUhP8JDApaWs_yM-zUQMq7zvVe2ogeqZtFVHgxeSUVhPTcB-EyR0Hdtjk8Uh0LtN9GiUjL5a1NlU1P_RyA6V0S8oUwJ45KGw',
            'Content-Type': 'application/json'
        }
    };

    // Enviando POST com body
    let res = http.post('http://localhost:5256/api/product/add', payload, params);

    check(res, { 'status é 200': (r) => r.status === 200 });

    sleep(1);
}
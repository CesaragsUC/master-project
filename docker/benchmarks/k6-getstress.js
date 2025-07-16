import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    vus: 200, // Número de usuários simultâneos
    duration: '10m', // Tempo do teste
};

export default function () {
    let params = {
        headers: {
            'Authorization': 'eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJHVlV0cHZ3ajdIRmI5WVNwVHZMY2F0dEM4YUpZU2J3bG1BQ0U3aFNLWHEwIn0.eyJleHAiOjE3NDI3MDM5OTksImlhdCI6MTc0MjY2Nzk5OSwianRpIjoiNjIxYmMxOGUtMDBhMi00Yzk1LWI0MDgtMTQ4OTc0NzcxMzhkIiwiaXNzIjoiaHR0cDovL2Nhc29mdC1zdG9yZS1rZXljbG9hazo4MDgwL3JlYWxtcy9jYXNvZnQiLCJhdWQiOlsiY2Fzb2Z0c3lzdGVtIiwiYWNjb3VudCJdLCJzdWIiOiI0NzM5MmI3Ny1mMDBmLTRhOGMtYTU3MC1iMWNlNWY1NTZlMjUiLCJ0eXAiOiJCZWFyZXIiLCJhenAiOiJjYXNvZnRzeXN0ZW0iLCJzaWQiOiI3NWZmMDViOC01MzQ3LTRmZjAtYTE5OS00Yzg4ZWFiY2NjYmUiLCJhY3IiOiIxIiwiYWxsb3dlZC1vcmlnaW5zIjpbIioiXSwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbIm9mZmxpbmVfYWNjZXNzIiwiZGVmYXVsdC1yb2xlcy1jYXNvZnQiLCJ1bWFfYXV0aG9yaXphdGlvbiJdfSwicmVzb3VyY2VfYWNjZXNzIjp7ImNhc29mdHN5c3RlbSI6eyJyb2xlcyI6WyJEZWxldGUiLCJSZWFkIiwiQ3JlYXRlIiwiQWRtaW4iLCJVcGRhdGUiXX0sImFjY291bnQiOnsicm9sZXMiOlsibWFuYWdlLWFjY291bnQiLCJtYW5hZ2UtYWNjb3VudC1saW5rcyIsInZpZXctcHJvZmlsZSJdfX0sInNjb3BlIjoib3BlbmlkIHByb2ZpbGUgZW1haWwiLCJlbWFpbF92ZXJpZmllZCI6ZmFsc2UsIm5hbWUiOiJDZXNhciBTYW50b3MiLCJwcmVmZXJyZWRfdXNlcm5hbWUiOiJjZXNhciIsImdpdmVuX25hbWUiOiJDZXNhciIsImZhbWlseV9uYW1lIjoiU2FudG9zIiwiZW1haWwiOiJjZXNhckBkZW1vLmNvbSJ9.IVxEvJU8EaIHLFl0HM5hVLBPa0i64cpz1inuDRNjqNXG3NM94jE5r1qWzHjQbkH-gI21KAqV-j14hLR_S-i8n-hxq2dfyWFjLonLlhIFKDj1JiPnD3XvZeYHxn8ZZCODuoqtYcODCYW4615NCff4JEIwoHM4E_4aX94aKowkm1sw9fSZAYbPtcNnPjcJoc2WXCAvykdpVQZ_KkwvLIcuTDQqx0dsWLg0oJBTtUyKsfc8xtVKyTpY_MSMk9odyxjA9KPtuXyn8TcNisueKinDvFLeL8umGD5mRjlXPob4IUTpZb_VQSp8lmY2ybmPBik9SsuAGSdtWY5WN_SuLSqtFA',
            'Content-Type': 'application/json'
        }
    };

    let res = http.get('http://localhost:5256/api/product/all', params);
    check(res, { 'status é 200': (r) => r.status === 200 });

    sleep(1);
}

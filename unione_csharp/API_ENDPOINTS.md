# UniOne API Endpoints - C# Implementation Reference

(Refer to `unione_node/API_ENDPOINTS.md` for the complete specification. The C# implementation will strictly follow these routes.)

## Authentication Endpoints
- `POST /api/auth/login`
- `POST /api/auth/logout`
- `GET /api/auth/me`

## Student Portal
- `GET /api/student/profile`
- `GET /api/student/grades`
- `POST /api/student/enrollments`

## Professor Portal
- `GET /api/professor/sections`
- `POST /api/professor/sections/{sectionId}/grades`

## Admin Portal
- `GET /api/admin/webhooks`
- `POST /api/admin/webhooks`

(Full list matches the Node.js implementation specification)

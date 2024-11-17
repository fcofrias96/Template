import { HttpInterceptorFn } from '@angular/common/http';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {

  const token  = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJqb2huRG9lIiwianRpIjoiODk0MTZlNmYtMzRkZS00NjY5LWI1YWUtMDlmZmYyYTdhNTY4IiwiZW1haWwiOiJqb2huRG9lQGdtYWlsLmNvbSIsInVzZXJJZCI6ImY3YTAyN2EwLTE5ZDktNGM1Ni04Zjg3LTM2YWI0YmI5OGU0NCIsInJvbGVzIjpbIkFkbWluIiwiVXNlciJdLCJleHAiOjE3MzE4NjI3NjUsImlzcyI6Imh0dHBzOi8vYXBpLnRlbXBsYXRlLmNvbSIsImF1ZCI6Imh0dHBzOi8vdGVtcGxhdGUuY29tIn0._2AYlPQzYNXzX53mQtht_8R5HywiEEauDJ0-A3g7qWU';

  //const token = localStorage.getItem('jwtToken');

  // Clonar la solicitud y agregar el token al encabezado Authorization si el token existe
  const authReq = token
    ? req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      })

    : req;

  return next(authReq);
};

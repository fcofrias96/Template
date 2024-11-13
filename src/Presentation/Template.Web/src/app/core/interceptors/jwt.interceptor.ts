import { HttpInterceptorFn } from '@angular/common/http';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {

  const token  = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJqb2huRG9lIiwianRpIjoiM2VkMDAxODktNzE4OS00Y2QzLWJmMjItZDgzNDlkNWM3MTEyIiwiZW1haWwiOiJqb2huRG9lQGdtYWlsLmNvbSIsInVzZXJJZCI6IjRmYTMzMTUzLWVjMzMtNDg1Ni1hYjRhLTI0MmExN2QxMDczMSIsInJvbGVzIjpbIlVzZXIiLCJBZG1pbiJdLCJleHAiOjE3MzEzMTg0NTcsImlzcyI6Imh0dHBzOi8vYXBpLnRlbXBsYXRlLmNvbSIsImF1ZCI6Imh0dHBzOi8vdGVtcGxhdGUuY29tIn0.beu6vmRixNC56t7NWJGbVTNGaVqHMN5JX9W86J1eO_E';

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

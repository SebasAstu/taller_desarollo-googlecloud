import {render, screen, waitFor} from '@testing-library/react';

import {rest} from 'msw';
import {setupServer} from 'msw/node';
import {act} from 'react-dom/test-utils';

import {MemoryRouter, Route, Routes} from 'react-router-dom'
import ShowFixedAssets from './ShowFixedAssets';

describe('Show Fixed Asset', () => {
  const fixedAssetUrl =process.env.REACT_APP_BACKEND_URL + '/api/fixedAssets'  
  const fixedAssetCategoriesUrl =process.env.REACT_APP_BACKEND_URL + '/api/assetCategories?showAssets=true'
  const programHousesUrl =process.env.REACT_APP_BACKEND_URL + '/api/programHouses'
  const statesUrl = process.env.REACT_APP_BACKEND_URL + '/api/assetStates'

  function getResponse(url, jsonData=null, code=200, text=null){
    const response = rest.get(url, (req, res, ctx) => {
      if(code!=200) return res(ctx.status(code), ctx.text(text))
      return res(ctx.json(jsonData))
    })
    return response
  }

  const programHouses =
  [
      {
          id: 1,
          acronym:"SDE"
      },
      {
          id: 1,
          acronym:"CAC"
      }
  ]

  const states =
  [
      {
          id: 1,
          acronym:"Bueno"
      },
      {
          id: 2,
          acronym:"Malo"
      },
      {
        id: 3,
        acronym:"Verificar"
    }
  ]

  const assetCategories =
  [
    {
        "id": 1,
        "category": "Equipos y Herramientas",
        "fixedAssets": []
    },
    {
        "id": 2,
        "category": "Muebles y Enseres",
        "fixedAssets": []
    },
    {
        "id": 4,
        "category": "Herramientas",
        "fixedAssets": []
    },
    {
        "id": 3,
        "category": "Maquinaria",
        "fixedAssets": []
    }
  ]

  const fixedAssets =
  [
    {
       "id":1,
       "name":"Asset name 1",
       "price":100.58,
       "quantity":5
    },
    {
       "id":2,
       "name":"Asset name 2",
       "price":100.58,
       "quantity":5
    },
    {
       "id":3,
       "name":"Asset name 3",
       "price":1000,
       "quantity":1
    },
    {
       "id":4,
       "name":"Asset name 4",
       "price":200,
       "quantity":1
    },
    {
       "id":5,
       "name":"Asset name 5",
       "price":100,
       "quantity":1
    }
  ]

  const fixedAssetsOnlyRequiredFields =
  [
    {
       "id":1,
       "code": "TEC-001",
       "name":"Asset name 1",
       "price":100,
       "quantity":1
    },
    {
       "id":2,
       "code": "TEC-002",
       "name":"Asset name 2",
       "price":100,
       "quantity":1
    },
    {
       "id":3,
       "code": "TEC-003",
       "name":"Asset name 3",
       "price":100,
       "quantity":1
    },
    {
       "id":4,
       "code": "TEC-004",
       "name":"Asset name 4",
       "price":100,
       "quantity":1
    },
    {
       "id":5,
       "code": "TEC-005",
       "name":"Asset name 5",
       "price":100,
       "quantity":1
    }
  ]

	const fixedAssetResponse = getResponse(fixedAssetUrl, fixedAssets)
  const fixedAssetCategoriesResponse = getResponse(fixedAssetCategoriesUrl, assetCategories)
  const programHousesResponse = getResponse(programHousesUrl, programHouses)
  const StateResponse = getResponse(statesUrl, states)
  const handlers = [fixedAssetResponse, fixedAssetCategoriesResponse, programHousesResponse, StateResponse];

  const server = new setupServer(...handlers);

  beforeAll(() => server.listen());
  afterEach(() => server.resetHandlers());
  afterAll(() => server.close());

  function renderWithRouter(componentToRender, pathToElement, mockedPath){
    render( 
      <MemoryRouter initialEntries={[mockedPath]}>
          <Routes>
              <Route path={pathToElement} element={componentToRender}></Route>
          </Routes>
      </MemoryRouter>
    )
  }

   it('Shows a list of fixed assets data correctly', async () => {
     act(()=>{
       renderWithRouter(<ShowFixedAssets/>,"/activos-fijos","/activos-fijos" )
     }) 
     await waitFor(() => {
         expect(screen.getByText('Lista de activos fijos')).toBeVisible
         expect(screen.getByText('Crear activo fijo')).toBeVisible
         expect(screen.getByText('Herramientas')).toBeVisible
         expect(screen.queryByText('Asset name 1')).toBeVisible
         expect(screen.queryByText('Asset name 2')).toBeVisible
         expect(screen.queryByText('Asset name 3')).toBeVisible
         expect(screen.queryByText('Asset name 4')).toBeVisible
         expect(screen.queryByText('Asset name 5')).toBeVisible
         //expect(screen.getAllByText('Programa: *Sin programa*')).toHaveLength(5)
       })  
   })
  
  it('Shows fixed assets data when non-required fields are null', async () => {    
    const fixedAssestWithOnlyRequiredFields = getResponse(fixedAssetUrl, fixedAssetsOnlyRequiredFields)
    server.use(fixedAssestWithOnlyRequiredFields)
    act(()=>{
      renderWithRouter(<ShowFixedAssets/>,"/activos-fijos","/activos-fijos" )
    }) 
    await waitFor(() => {
      expect(screen.getByText('Herramientas')).toBeVisible
      expect(screen.queryByText('Asset name 1')).toBeVisible
      expect(screen.queryByText('Asset name 2')).toBeVisible
      expect(screen.queryByText('Asset name 3')).toBeVisible
      expect(screen.queryByText('Asset name 4')).toBeVisible
      expect(screen.queryByText('Asset name 5')).toBeVisible
      //expect(screen.queryAllByText('Programa: *Sin programa*')).toHaveLength(5)
      })  
  })
  /*
  it('Shows error when api does not return any data. Should return error 500', async () => {
    const fixedAssetInternalServiceErrorResponse = getResponse(fixedAssetUrl, null, 500, "Lo sentimos, algo sucedió.")
    server.use(fixedAssetInternalServiceErrorResponse)
    act(()=>{
      renderWithRouter(<ShowFixedAssets/>,"/activos-fijos","/activos-fijos" )
    }) 
    await waitFor(() => {
        expect(screen.getByText("ERROR 500: Lo sentimos, algo sucedió.").toBeVisible)
      })  
  });
  */
})
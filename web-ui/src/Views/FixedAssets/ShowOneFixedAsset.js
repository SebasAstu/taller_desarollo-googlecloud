import React, { useState } from 'react'
import { useParams, useNavigate, useLocation } from 'react-router-dom'
import GetFromApi, { deleteFixedAssets } from '../../Components/GetFromApi'
import ErrorPage from '../../Components/ErrorPage'
import Navbar from '../../Components/NavBar'
import SingleItemCard from '../../Components/SingleItemCard'
import { ButtonDanger, ButtonSecondary, ButtonPrimaryEditIcon, ButtonPrimaryDeleteIcon } from '../../Components/MUI-Button';
import Box from '@mui/material/Box'
import Alert from '@mui/material/Alert';
import { Snackbar } from '@mui/material';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import DialogContentText from '@mui/material/DialogContentText';

let accesPermiss = sessionStorage.getItem("Access")

export function ShowFixedAsset() {
    const { fixedAssetId } = useParams()
    const location = useLocation()
    let showAlert = location.state ? location.state.showAlert : false 
    let alertMessage = location.state ? location.state.alertMessage : null 
    const [open, setOpen] = useState(showAlert);
    const url = process.env.REACT_APP_BACKEND_URL + `/api/fixedAssets/${fixedAssetId}`
    const { apiData:fixedAsset, error } = GetFromApi(url)
    let imageUrl = "https://st.depositphotos.com/1005574/2080/v/450/depositphotos_20808761-stock-illustration-laptop.jpg" 
    const navigate = useNavigate();
    const navigateUpdateFixedAsset = () => { navigate(`/activos-fijos/${fixedAssetId}/editar-activo-fijo`); }
    const [openToConfirm, setOpenToConfirm] = useState(false);
    if(error){
        return ErrorPage(error)
    }
    if (!fixedAsset) return null
    const fixedAssetData = {
        "Tipo de Activo Fijo": fixedAsset.assetTypeAssetCategoryCategory,
        "Tipo": fixedAsset.assetTypeType,
        "Ubicación": fixedAsset.location,
        "Valor": fixedAsset.price,
        "Estado": fixedAsset.assetStateState,
        "Responsable": fixedAsset.assetResponsibleName
    }

    const fetchDeleteFixedAsset = () => {
        deleteFixedAssets(url)
        .then((response) => {
            if (response.status == 200){
                navigate(`/activos-fijos`,{state:{showAlert:true,alertMessage:"Activo Fijo Eliminado"}})
            }
        })
    }

    function handleCloseToConfirm(event, reason) {
        if (reason === 'clickaway') {
            return
        }
        setOpenToConfirm(false)
    }

    function handleClose(event, reason) {
        if (reason === 'clickaway') {
            return
        }
        setOpen(false)
    }
    
    const ToConfirmOpen = () => {
        handleCloseToConfirm();
        setOpenToConfirm(true);
    };
    const buttonsList = accesPermiss == "CompleteAccess" ? (
        <Box sx={{alignSelf:'flex-end', display:'flex-end'}}>
            <ButtonPrimaryEditIcon id="edit_button" onClick={navigateUpdateFixedAsset} sx={{marginLeft:1, alignSelf:'flex-end'}}/>
            <ButtonPrimaryDeleteIcon id="delete_button" onClick={ToConfirmOpen} sx={{marginLeft:1, alignSelf:'flex-end'}}/>
        </Box>) : null
    
    return (
        <>
            <Navbar />
            <div style={{ marginTop: '11vh', display:'flex', flexDirection:'column', justifyContent:'center', alignItems:'center' }}>
            <SingleItemCard title={fixedAsset.code ? `${fixedAsset.name} #${fixedAsset.code}` : `${fixedAsset.name}`} secondaryField={fixedAsset.programHouseName} element={fixedAssetData} itemsPerLine={3} imageUrl={imageUrl} imageCirle={false} imgHeight={300} imgWidth={500} button={buttonsList} />        
            <Snackbar open={open} autoHideDuration={6000} onClose={handleClose}>
                <Alert onClose={handleClose} severity="success">
                    {alertMessage}
                </Alert>
            </Snackbar>
            <Dialog open={openToConfirm} onClose={handleCloseToConfirm} id="confirmation_popup" sx={{borderRadius:3 }}>
                <DialogTitle sx={{display:'flex', justifyContent:'center'}}>Eliminar</DialogTitle>
                <DialogContent>
                    <DialogContentText id="alert-dialog-description">
                        ¿Desea eliminar todos los datos de este activo fijo {fixedAsset.name}?
                    </DialogContentText>
                </DialogContent>
                <DialogActions sx={{display:'flex',flexDirection:'row', alignItems:'center', justifyContent:'center'}}>
                    <ButtonSecondary label="Cancelar" onClick={handleCloseToConfirm} sx={{alignSelf:'flex-end'}}></ButtonSecondary>
                    <ButtonDanger label="Eliminar" id="confirm_delete_fixed_asset_button" onClick={fetchDeleteFixedAsset} sx={{alignSelf:'flex-end'}}></ButtonDanger>
                </DialogActions>
            </Dialog>
            </div>
        </>
        )
}
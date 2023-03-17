import List from '@mui/material/List';
import ListElement from './ListElement'

export default function ListBasic({items, withImage=true, withDeleteIcon=false}) {
  return (
    <List sx={{ width: '100%', bgcolor: 'background.paper', alignItems :"flex-start" }}>
      {items.map((n,i)=>{
        return (<>
            <ListElement sx={{ width: '10%'}} key={n.id ? n.id : i} id={n.id} title={n.title} description={n.description} elementUrl={n.elementUrl} imgSrc={n.imgSrc} withImage={withImage} withDeleteIcon={withDeleteIcon}/>
        </>)})}
    </List>
  )};
const toJson = (item) => {
    return {
        id: item.id,
        name: item.name,
        soCMND: item.soCMND,
        publicationDate : item.publicationDate,
        sdt : item.sdt,
    }
}

const fromJson = (item) => {
    return {
        id: item.id,
        name: item.name,
        soCMND: item.soCMND,
        publicationDate : item.publicationDate,
        sdt : item.sdt,
    }
}

const baseJson = () => {
    return {
        id: null,
        name: null,
        soCMND: null,
        publicationDate : null,
        sdt : null
    }
}

const toListModel = (items) =>{
    if(items.length > 0){
        let data = [];
        items.map((value, index) =>{
            data.push(fromJson(value));
        })
        return data??[];
    }
    return [];
}
export const nguoiMuaModel = {
    toJson, fromJson, baseJson, toListModel
}

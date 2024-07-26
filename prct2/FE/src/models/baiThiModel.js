const toJson = (item) => {
    return {
        id: item.id,
        name: item.name, // tieu de
        moTaNgan : item.moTaNgan,
        noiDung : item.noiDung,
        hinhAnh: item.hinhAnh
        
    }
}

const fromJson = (item) => {
    return {
        id: item.id,
        name: item.name, // tieu de
        moTaNgan : item.moTaNgan,
        noiDung : item.noiDung,
        hinhAnh: item.hinhAnh
    }
}

const baseJson = () => {
    return {
        id: null,
        name: null,
        moTaNgan : null,
        noiDung : null,
        hinhAnh: null
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
export const baiThiModel = {
    toJson, fromJson, baseJson, toListModel
}

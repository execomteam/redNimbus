import React from 'react';
import Folder from './Folder';

class FolderGroup extends React.Component {
    constructor(props) {
        super(props);
    }

    render() {
        return(
            <div class="card-group">
                {
                    this.props.content.map((value, index) => {
                        return <Folder name={value}/>
                    })
                }
            </div>
        );
    }
}
export default FolderGroup;


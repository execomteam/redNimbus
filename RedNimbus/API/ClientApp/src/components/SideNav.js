import React from 'react';

class SideNav extends React.Component{
    render(){
        return (
            <div>
                <button style={{marginBottom: '20px'}} type="button" className="btn btn-danger" data-toggle="modal" data-target="#exampleModal">Create new bucket</button>

                <div className="modal fade" id="exampleModal" tabIndex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
                    <div className="modal-dialog" role="document">
                        <div className="modal-content">
                        <div className="modal-header">
                            <h5 className="modal-title" id="exampleModalLabel">Modal title</h5>
                            <button type="button" className="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div className="modal-body">
                            ...
                        </div>
                        <div className="modal-footer">
                            <button type="button" className="btn btn-secondary" data-dismiss="modal">Close</button>
                            <button type="button" className="btn btn-primary">Save changes</button>
                        </div>
                        </div>
                    </div>
                </div>

                <ul className="nav flex-column">
                    <li className="nav-item">
                        <a className="nav-link active" href="/bucket">My Buckets</a>
                    </li>
                    <li className="nav-item">
                        <a className="nav-link" href="#">Shared with me</a>
                    </li>
                </ul>
            </div>
        );
    }
}

export default SideNav;

 